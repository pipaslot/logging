using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Aggregators
{
    /// <summary>
    /// Basic abstraction of Queue handling messages and scopes
    /// </summary>
    internal abstract class QueueAggregatorBase : IQueueAggregator
    {
        private readonly IOptions<PipaslotLoggerOptions> _options;
        protected readonly QueueCollection Queues = new QueueCollection();

        protected QueueAggregatorBase(IOptions<PipaslotLoggerOptions> options)
        {
            _options = options;
        }

        protected abstract ILogWriter Writer { get; }

        public virtual void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            var canCreate = CanCreateNewLogScope(traceIdentifier, categoryName, severity);
            var queue = Queues.GetQueue(traceIdentifier, canCreate);
            if (queue == null){
                // LogRecord should be omitted
                return;
            }

            // Check if can be written for current scope and method
            if (!CanAddIntoExistingLogScope(traceIdentifier, categoryName, severity, queue)) return;

            queue.Add(new Record(categoryName, severity, message, state, queue.Depth, RecordType.Record));
            if (queue.Logs.Count == 1)
            {
                // Remove request history from memory 
                Queues.Remove(traceIdentifier);
                WriteQueue(queue);
            }
        }

        public virtual void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state)
        {
            var canCreate = CanCreateNewLogScope(traceIdentifier, categoryName, LogLevel.None);
            var queue = Queues.GetQueue(traceIdentifier, canCreate);
            if (queue == null){
                // LogRecord should be omitted
                return;
            }

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
            var processed = ProcessQueueBeforeWrite(queue);
            if(processed.HasAnyWriteableLog()){
                Writer.WriteLog(processed);
            }
        }
        
        protected virtual bool CanCreateNewLogScope(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return true;
        }

        protected virtual bool CanAddIntoExistingLogScope(string traceIdentifier, string categoryName, LogLevel severity, Queue queue)
        {
            return true;
        }

        protected abstract Queue ProcessQueueBeforeWrite(Queue queue);

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