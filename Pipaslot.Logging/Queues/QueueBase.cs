using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Groups;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Queues
{
    public abstract class QueueBase : IQueue
    {
        private readonly IOptions<PipaslotLoggerOptions> _options;
        protected readonly LogGroupFormatter Formatter = new LogGroupFormatter();
        protected readonly LogGroupCollection LogGroups = new LogGroupCollection();

        protected QueueBase(IOptions<PipaslotLoggerOptions> options)
        {
            _options = options;
        }

        protected abstract ILogWriter Writer { get; }

        public virtual void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            var canCreate = CanCreateNewQueue(traceIdentifier, categoryName, severity);
            var queue = LogGroups.GetQueue(traceIdentifier, canCreate);
            if (queue == null){
                // Log should be omitted
                return;
            }

            // Check if can be written for current scope and method
            if (!CanWriteIntoExistingQueue(categoryName, severity)) return;

            queue.Add(new LogGroup.Log(categoryName, severity, message, state, queue.Depth, true));
        }

        public virtual void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state)
        {
            var canCreate = CanCreateNewQueue(traceIdentifier, categoryName, LogLevel.None);
            var queue = LogGroups.GetQueue(traceIdentifier, canCreate);
            if (queue == null){
                // Log should be omitted
                return;
            }

            // Update depth
            var depth = queue.Depth;
            var stateType = typeof(TState);
            if (stateType == typeof(IncreaseScopeState) || stateType == typeof(IncreaseMethodState))
                depth++;
            else if (stateType == typeof(DecreaseScopeState)) depth--;

            // Log or finish
            if (depth <= 0){
                // Remove request history from memory 
                LogGroups.Remove(traceIdentifier);

                var log = Formatter.FormatRequest(queue, traceIdentifier);
                if (!string.IsNullOrWhiteSpace(log)) Writer.WriteLog(log, queue.Time.DateTime, traceIdentifier);
            }
            else{
                //Write only increasing scopes and ignore decreasing scopes
                var canWrite = CanWriteScope(state);
                queue.Add(new LogGroup.Log(categoryName, LogLevel.None, "", state, depth, canWrite));
            }
        }

        public virtual void Dispose()
        {
            //write all remaining logs
            foreach (var pair in LogGroups.GetAllQueues()){
                var log = Formatter.FormatRequest(pair.Value, pair.Key);
                if (!string.IsNullOrWhiteSpace(log)) Writer.WriteLog(log, pair.Value.Time.DateTime, pair.Key);
            }

            LogGroups.Dispose();
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