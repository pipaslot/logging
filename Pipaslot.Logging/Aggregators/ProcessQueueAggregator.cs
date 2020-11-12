using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Aggregators
{
    /// <summary>
    ///     LogRecord all processes together
    /// </summary>
    internal class ProcessQueueAggregator : IQueueAggregator
    {
        
        private readonly IOptions<PipaslotLoggerOptions> _options;
        protected readonly QueueCollection Queues = new QueueCollection();
        private readonly ILogWriter _writer;

        public ProcessQueueAggregator(ILogWriter writer, IOptions<PipaslotLoggerOptions> options)
        {
            _writer = writer;
            _options = options;
        }

        public void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            if (CanBeWrittenIntoQueue(traceIdentifier))
            {
                var queue = Queues.GetQueue(traceIdentifier, false);
                if (queue == null)
                {
                    // Write directly if is nto part of some queue bordered by scope
                    var group = new Queue(traceIdentifier);
                    group.Add(new Record(categoryName, severity, message, state, group.Depth, RecordType.Record));
                    _writer.WriteLog(group);
                    return;
                }

                queue.Add(new Record(categoryName, severity, message, state, queue.Depth, RecordType.Record));
            }
        }

        public void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state)
        {
            if (CanBeWrittenIntoQueue(traceIdentifier))
            {
                var queue = Queues.GetQueue(traceIdentifier, true);
                if (queue == null){
                    // LogRecord should be omitted
                    return;
                }

                // Update depth
                var depth = queue.Depth;
                var logType = QueueAggregatorBase.GetLogType<TState>(_options.Value);
                if (logType == RecordType.ScopeBegin || logType == RecordType.ScopeBeginIgnored){
                    depth++;
                }
                else if (logType == RecordType.ScopeEnd){
                    depth--;
                }

                // LogRecord or finish
                if (depth <= 0){
                    // Remove request history from memory 
                    Queues.Remove(traceIdentifier);
                    if(queue.HasAnyWriteableLog()){
                        _writer.WriteLog(queue);
                    }
                }
                else{
                    //Write only increasing scopes and ignore decreasing scopes
                    queue.Add(new Record(categoryName, LogLevel.None, "", state, depth, logType));
                }
            }
        }

        private bool CanBeWrittenIntoQueue(string traceIdentifier)
        {
            return traceIdentifier?.StartsWith(Constants.CliTraceIdentifierPrefix) ?? false;
        }
        
        public void Dispose()
        {
            //write all remaining logs
            foreach (var pair in Queues.GetAllQueues()){
                if(pair.Value.HasAnyWriteableLog()){
                    _writer.WriteLog(pair.Value);
                }
            }

            Queues.Dispose();
        }
    }
}