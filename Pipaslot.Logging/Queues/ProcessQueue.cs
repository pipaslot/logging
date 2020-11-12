using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Records;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    ///     LogRecord all processes together
    /// </summary>
    internal class ProcessQueue : IQueue
    {
        
        private readonly IOptions<PipaslotLoggerOptions> _options;
        protected readonly LogScopeCollection LogScopes = new LogScopeCollection();
        private readonly ILogWriter _writer;

        public ProcessQueue(ILogWriter writer, IOptions<PipaslotLoggerOptions> options)
        {
            _writer = writer;
            _options = options;
        }

        public void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            if (CanWrite(traceIdentifier))
            {
                var queue = LogScopes.GetQueue(traceIdentifier, false);
                if (queue == null)
                {
                    // Write directly if is nto part of some queue bordered by scope
                    var group = new LogScope(traceIdentifier);
                    group.Add(new LogRecord(categoryName, severity, message, state, group.Depth, true));
                    _writer.WriteLog(group);
                    return;
                }

                queue.Add(new LogRecord(categoryName, severity, message, state, queue.Depth, true));
            }
        }

        public void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state)
        {
            if (CanWrite(traceIdentifier))
            {
                var queue = LogScopes.GetQueue(traceIdentifier, true);
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
                    _writer.WriteLog(queue);
                }
                else{
                    //Write only increasing scopes and ignore decreasing scopes
                    var canWrite = CanWriteScope(state);
                    queue.Add(new LogRecord(categoryName, LogLevel.None, "", state, depth, canWrite));
                }
            }
        }
        private bool CanWriteScope<TState>(TState state)
        {
            var options = _options.Value;
            if (state is IncreaseMethodState && options.IncludeMethods) return true;
            if (options.IncludeScopes) return true;

            return false;
        }

        private bool CanWrite(string traceIdentifier)
        {
            return traceIdentifier?.StartsWith(Constants.CliTraceIdentifierPrefix) ?? false;
        }

        public void Dispose()
        {
            //write all remaining logs
            foreach (var pair in LogScopes.GetAllQueues()){
                _writer.WriteLog(pair.Value);
            }

            LogScopes.Dispose();
        }
    }
}