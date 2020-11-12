using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Records;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Basic abstraction of Queue handling messages and scopes
    /// </summary>
    internal abstract class QueueBase : IQueue
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
            var canCreate = CanCreateNewLogScope(traceIdentifier, categoryName, severity);
            var queue = LogScopes.GetQueue(traceIdentifier, canCreate);
            if (queue == null){
                // LogRecord should be omitted
                return;
            }

            // Check if can be written for current scope and method
            if (!CanAddIntoExistingLogScope(categoryName, severity, queue)) return;

            queue.Add(new LogRecord(categoryName, severity, message, state, queue.Depth, LogType.Record));
        }

        public virtual void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state)
        {
            var canCreate = CanCreateNewLogScope(traceIdentifier, categoryName, LogLevel.None);
            var queue = LogScopes.GetQueue(traceIdentifier, canCreate);
            if (queue == null){
                // LogRecord should be omitted
                return;
            }

            // Update depth
            var depth = queue.Depth;
            var logType = GetLogType<TState>(_options.Value);
            if (logType == LogType.ScopeBegin || logType == LogType.ScopeBeginIgnored){
                depth++;
            }
            else if (logType == LogType.ScopeEnd){
                depth--;
            }

            // LogRecord or finish
            if (depth <= 0){
                // Remove request history from memory 
                LogScopes.Remove(traceIdentifier);
                if(queue.HasAnyWriteableLog()){
                    Writer.WriteLog(queue);
                }
            }
            else{
                //Write only increasing scopes and ignore decreasing scopes
                queue.Add(new LogRecord(categoryName, LogLevel.None, "", state, depth, logType));
            }
        }

        public virtual void Dispose()
        {
            //write all remaining logs
            foreach (var pair in LogScopes.GetAllQueues()){
                if(pair.Value.HasAnyWriteableLog()){
                    Writer.WriteLog(pair.Value);
                }
            }

            LogScopes.Dispose();
        }

        protected abstract bool CanCreateNewLogScope(string traceIdentifier, string categoryName, LogLevel severity);

        protected abstract bool CanAddIntoExistingLogScope(string categoryName, LogLevel severity, LogScope scope);

        internal static LogType GetLogType<TState>(PipaslotLoggerOptions options)
        {
            var stateType = typeof(TState);
            if (stateType == typeof(IncreaseScopeState)){
                return options.IncludeScopes ? LogType.ScopeBegin : LogType.ScopeBeginIgnored;
            }
            if (stateType == typeof(IncreaseMethodState)){
                return options.IncludeMethods ? LogType.ScopeBegin : LogType.ScopeBeginIgnored;
            }
            if (stateType == typeof(DecreaseScopeState)){
                return LogType.ScopeEnd;
            }
            return LogType.Record;
        }
    }
}