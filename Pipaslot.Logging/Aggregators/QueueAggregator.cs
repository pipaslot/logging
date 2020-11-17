using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Filters;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Aggregators
{
    /// <summary>
    /// Basic abstraction of Queue handling messages and scopes
    /// </summary>
    internal class QueueAggregator : IQueueAggregator
    {
        private readonly IOptions<PipaslotLoggerOptions> _options;
        private readonly IQueueFilter _queueFilter;
        private readonly ILogWriter _writer;
        protected readonly QueueCollection Queues = new QueueCollection();

        public QueueAggregator(ILogWriter writer, IOptions<PipaslotLoggerOptions> options, IQueueFilter queueFilter)
        {
            _options = options;
            _queueFilter = queueFilter;
            _writer = writer;
        }
        
        public virtual void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            var queue = Queues.GetOrCreateQueue(traceIdentifier);
            
            queue.Add(new Record(categoryName, severity, message, state, queue.Depth, RecordType.Record));
            if (queue.Count == 1)
            {
                // Remove request history from memory 
                Queues.Remove(traceIdentifier);
                WriteQueue(queue);
            }
        }

        public virtual void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state)
        {
            var queue = Queues.GetOrCreateQueue(traceIdentifier);

            // Update depth
            var depth = queue.Depth;
            var logType = GetLogType<TState>(_options.Value);
            if (logType == RecordType.ScopeBegin || logType == RecordType.ScopeBeginIgnored){
                depth++;
            }
            else if (logType == RecordType.ScopeEndIgnored){
                depth--;
            }
            queue.Add(new Record(categoryName, LogLevel.None, "", state, depth, logType));

            // LogRecord or finish
            if (depth <= 0){
                // Remove request history from memory 
                Queues.Remove(traceIdentifier);
                WriteQueue(queue);
            }
        }

        public virtual void Dispose()
        {
            //write all remaining logs
            foreach (var pair in Queues.GetAllQueues()){
                WriteQueue(pair.Value);
            }

            Queues.Dispose();
        }

        private void WriteQueue(Queue queue)
        {
            var processed = _queueFilter.Filter(queue);
            if(processed.HasAnyRecord()){
                _writer.WriteLog(processed);
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