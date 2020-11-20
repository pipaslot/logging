using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Aggregators
{
    /// <summary>
    /// Basic abstraction of Queue handling messages and scopes
    /// </summary>
    public class QueueAggregator
    {
        private readonly IEnumerable<Pipe> _pipes;
        private readonly IOptions<PipaslotLoggerOptions> _options;
        private readonly QueueCollection _queues = new QueueCollection();

        public QueueAggregator(IEnumerable<Pipe> pipes, IOptions<PipaslotLoggerOptions> options)
        {
            _pipes = pipes;
            _options = options;
        }
        
        public virtual void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            var queue = _queues.GetQueueOrNull(traceIdentifier);
            if (queue != null)
            {
                queue.Add(new Record(categoryName, severity, message, state, queue.Depth, RecordType.Record));
            }
            else
            {
                WriteQueue(new FixedSizeQueue(traceIdentifier, new Record(categoryName, severity, message, state, 0, RecordType.Record)));
            }
        }

        public virtual void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state)
        {
            var queue = _queues.GetOrCreateQueue(traceIdentifier);

            // Update depth
            var depth = queue.Depth;
            var logType = GetLogType<TState>(_options.Value);
            if (logType == RecordType.ScopeBegin || logType == RecordType.ScopeBeginIgnored){
                depth++;
                queue.Add(new Record(categoryName, LogLevel.None, "", state, depth, logType));
            }
            else if (logType == RecordType.ScopeEndIgnored){
                queue.Add(new Record(categoryName, LogLevel.None, "", state, depth, logType));
                depth--;
            }
            else
            {
                queue.Add(new Record(categoryName, LogLevel.None, "", state, depth, logType));
            }
            

            // LogRecord or finish
            if (depth <= 0){
                // Remove request history from memory 
                _queues.Remove(traceIdentifier);
                WriteQueue(queue);
            }
        }

        public virtual void Dispose()
        {
            //write all remaining logs
            foreach (var pair in _queues.GetAllQueues()){
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
        
        private static RecordType GetLogType<TState>(PipaslotLoggerOptions options)
        {
            var stateType = typeof(TState);
            if (stateType == typeof(IncreaseScopeState)){
                return options.IncludeScopes ? RecordType.ScopeBegin : RecordType.ScopeBeginIgnored;
            }
            if (stateType == typeof(IncreaseMethodState)){
                return options.IncludeMethods ? RecordType.ScopeBegin : RecordType.ScopeBeginIgnored;
            }
            if (stateType == typeof(DecreaseScopeState)){
                return RecordType.ScopeEndIgnored;
            }
            return RecordType.Record;
        }
    }
}