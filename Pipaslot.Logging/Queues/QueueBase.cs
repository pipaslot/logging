using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Records;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Basic abstraction of Queue handling messages and scopes
    /// </summary>
    public abstract class QueueBase : IQueue
    {
        private readonly IOptions<PipaslotLoggerOptions> _options;
        protected readonly LogScopeCollection LogScopes = new LogScopeCollection();

        protected QueueBase(IOptions<PipaslotLoggerOptions> options)
        {
            _options = options;
        }

        protected abstract ILogWriter Writer { get; }

        public virtual void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            var canCreate = CanCreateNewQueue(traceIdentifier, categoryName, severity);
            var queue = LogScopes.GetQueue(traceIdentifier, canCreate);
            if (queue == null){
                // LogRecord should be omitted
                return;
            }

            // Check if can be written for current scope and method
            if (!CanWriteIntoExistingQueue(categoryName, severity)) return;

            queue.Add(new LogRecord(categoryName, severity, message, state, queue.Depth, true));
        }

        public virtual void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state)
        {
            var canCreate = CanCreateNewQueue(traceIdentifier, categoryName, LogLevel.None);
            var queue = LogScopes.GetQueue(traceIdentifier, canCreate);
            if (queue == null){
                // LogRecord should be omitted
                return;
            }

            // Update depth
            var depth = queue.Depth;
            var stateType = typeof(TState);
            if (stateType == typeof(IncreaseScopeState) || stateType == typeof(IncreaseMethodState))
                depth++;
            else if (stateType == typeof(DecreaseScopeState)) depth--;

            // LogRecord or finish
            if (depth <= 0){
                // Remove request history from memory 
                LogScopes.Remove(traceIdentifier);
                Writer.WriteLog(queue);
            }
            else{
                //Write only increasing scopes and ignore decreasing scopes
                var canWrite = CanWriteScope(state);
                queue.Add(new LogRecord(categoryName, LogLevel.None, "", state, depth, canWrite));
            }
        }

        public virtual void Dispose()
        {
            //write all remaining logs
            foreach (var pair in LogScopes.GetAllQueues()){
                Writer.WriteLog(pair.Value);
            }

            LogScopes.Dispose();
        }

        protected abstract bool CanCreateNewQueue(string traceIdentifier, string categoryName, LogLevel severity);

        protected abstract bool CanWriteIntoExistingQueue(string categoryName, LogLevel severity);

        private bool CanWriteScope<TState>(TState state)
        {
            var options = _options.Value;
            if (state is IncreaseMethodState && options.IncludeMethods) return true;
            if (options.IncludeScopes) return true;

            return false;
        }
    }
}