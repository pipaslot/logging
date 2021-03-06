﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Configuration;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Aggregators
{
    /// <summary>
    ///     Basic abstraction of Queue handling messages and scopes
    /// </summary>
    public class QueueAggregator
    {
        internal static bool CanCreateQueueFromScopes = true;
        private readonly IOptions<PipaslotLoggerOptions> _options;
        private readonly IEnumerable<Pipe> _pipes;
        private readonly QueueCollection _queues = new QueueCollection();

        public QueueAggregator(IEnumerable<Pipe> pipes, IOptions<PipaslotLoggerOptions> options)
        {
            _pipes = pipes;
            _options = options;
        }

        /// <summary>
        /// Force queue creation
        /// </summary>
        internal void BeginQueue(string traceIdentifier)
        {
            _queues.CreateQueue(traceIdentifier);
        }

        /// <summary>
        ///  Force queue finishing. Write all logs aggregated for specified trace identifier
        /// </summary>
        internal void EndQueue(string traceIdentifier)
        {
            var queue = _queues.GetQueueOrNull(traceIdentifier);
            if (queue == null)
            {
                return;
            }

            _queues.Remove(traceIdentifier);
            WriteQueue(queue);
            WriteUnfinishedQueues();
        }

        public virtual void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            var writeScopesByInternalDetection = CanWriteScopesByInternalDetection(traceIdentifier);
            var queue = _queues.GetQueueOrNull(traceIdentifier);
            if (queue != null)
            {
                queue.Add(new Record(categoryName, severity, message, state, queue.Depth, RecordType.Record), queue.Depth);
            }
            else if (writeScopesByInternalDetection)
            {
                WriteQueue(new FixedSizeQueue(traceIdentifier, new Record(categoryName, severity, message, state, 0, RecordType.Record)));
            }
        }

        internal virtual void WriteScopeChange(string traceIdentifier, string categoryName, IState state)
        {
            var queue = _queues.GetQueueOrNull(traceIdentifier);
            var writeScopesByInternalDetection = CanWriteScopesByInternalDetection(traceIdentifier);
            if (queue == null)
            {
                if (writeScopesByInternalDetection)
                {
                    queue = _queues.CreateQueue(traceIdentifier);
                }
                else
                {
                    return;
                }
            }
            // Update depth
            var depth = queue.Depth;
            var logType = GetLogType(state.GetType(), _options.Value);
            if (logType == RecordType.ScopeBegin || logType == RecordType.ScopeBeginIgnored)
            {
                depth++;
                queue.Add(new Record(categoryName, LogLevel.None, "", state, depth, logType), depth, state);
            }
            else if (logType == RecordType.ScopeEndIgnored)
            {
                if (queue.IncreaseScopeState == ((DecreaseScopeState)state).IncreaseScopeState)
                {
                    // Some decrease scope was missing but we detected end of first scope
                    depth = 1;
                }

                queue.Add(new Record(categoryName, LogLevel.None, "", state, depth, logType), depth - 1);
            }
            else
                queue.Add(new Record(categoryName, LogLevel.None, "", state, depth, logType));


            // LogRecord or finish
            if (queue.Depth <= 0 && writeScopesByInternalDetection)
            {
                // Remove request history from memory 
                _queues.Remove(traceIdentifier);
                WriteQueue(queue);

                WriteUnfinishedQueues();
            }
        }
        
        private bool CanWriteScopesByInternalDetection(string traceIdentifier)
        {
            return CanCreateQueueFromScopes || traceIdentifier.StartsWith(Constants.CliTraceIdentifierPrefix);
        }

        private void WriteUnfinishedQueues()
        {
            var maxAxe = DateTimeOffset.Now - TimeSpan.FromHours(1);
            var queues = _queues.GetQueues(3, maxAxe);
                
            foreach (var pair in queues)
            {
                _queues.Remove(pair.Key);
                WriteQueue(pair.Value);
            }
        }

        public virtual void Dispose()
        {
            //write all remaining logs
            foreach (var pair in _queues.GetAllQueues())
            {
                WriteQueue(pair.Value);
            }

            _queues.Dispose();
        }

        private void WriteQueue(IQueue queue)
        {
            foreach (var pipe in _pipes)
            {
                pipe.Process(queue);
            }
        }

        private static RecordType GetLogType(Type stateType, PipaslotLoggerOptions options)
        {
            if (stateType == typeof(IncreaseScopeState)) return options.IncludeScopes ? RecordType.ScopeBegin : RecordType.ScopeBeginIgnored;
            if (stateType == typeof(IncreaseMethodState)) return options.IncludeMethods ? RecordType.ScopeBegin : RecordType.ScopeBeginIgnored;
            if (stateType == typeof(DecreaseScopeState)) return RecordType.ScopeEndIgnored;
            return RecordType.Record;
        }
    }
}