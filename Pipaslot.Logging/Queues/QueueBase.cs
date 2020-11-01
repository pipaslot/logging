using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Groups;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Queues
{
    public abstract class QueueBase : IQueue
    {
        protected readonly LogGroupCollection LogGroups = new LogGroupCollection();
        protected readonly LogGroupFormatter Formatter = new LogGroupFormatter();
        private readonly IOptions<PipaslotLoggerOptions> _options;

        protected QueueBase(IOptions<PipaslotLoggerOptions> options)
        {
            _options = options;
        }

        protected abstract bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName, LogLevel severity, TState state);

        protected abstract bool CanWrite<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state);
        protected abstract ILogWriter Writer { get; }
        public virtual void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message,
            TState state)
        {
            var canCreate = CanCreateNewQueue(traceIdentifier, categoryName, severity, state);
            var queue = LogGroups.GetQueue(traceIdentifier, canCreate);
            if (queue == null)
            {
                // Log should be ommited
                return;
            }

            // Check if can be written for current scope and method
            if (!CanWrite(traceIdentifier, categoryName, severity, message, state))
            {
                return;
            }
            
            queue.Add(new LogGroup.Log(categoryName, severity, message, state, queue.Depth, true));
        }

        public virtual void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state)
        {
            var canCreate = CanCreateNewQueue(traceIdentifier, categoryName, LogLevel.Trace, state);
            var queue = LogGroups.GetQueue(traceIdentifier, canCreate);
            if (queue == null)
            {
                // Log should be ommited
                return;
            }
            
            // Update depth
            var depth = queue.Depth;
            var stateType = typeof(TState);
            if (stateType == typeof(IncreaseScopeState) || stateType == typeof(IncreaseMethodState))
            {
                depth++;
            }
            else if (stateType == typeof(DecreaseScopeState))
            {
                depth--;
            }

            // Log or finish
            if (depth <= 0)
            {
                // Remove request history from memory 
                LogGroups.Remove(traceIdentifier);
                
                var log = Formatter.FormatRequest(queue, traceIdentifier);
                Writer.WriteLog(log, queue.Time.DateTime, traceIdentifier);
            }
            else{
                //Write only increasing scopes and ignore decreasing scopes
                var canWrite = CanWriteScope(state);
                queue.Add(new LogGroup.Log(categoryName, LogLevel.Trace, "", state, depth, canWrite));
            }
        }

        private bool CanWriteScope<TState>(TState state)
        {
            var options = _options.Value;
            if (state is IncreaseMethodState && options.IncludeMethods){
                return true;
            }
            if (options.IncludeScopes){
                return true;
            }

            return false;
        }

        public virtual void Dispose()
        {
            //write all remaining logs
            foreach (var pair in LogGroups.GetAllQueues())
            {
                var log = Formatter.FormatRequest(pair.Value, pair.Key);
                Writer.WriteLog(log, pair.Value.Time.DateTime, pair.Key);
            }
            LogGroups.Dispose();
        }
    }
}