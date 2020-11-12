using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Groups;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    ///     Log all processes together
    /// </summary>
    public class ProcessQueue : IQueue
    {
        
        private readonly IOptions<PipaslotLoggerOptions> _options;
        protected readonly LogGroupFormatter Formatter = new LogGroupFormatter();
        protected readonly LogGroupCollection LogGroups = new LogGroupCollection();
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
                var queue = LogGroups.GetQueue(traceIdentifier, false);
                if (queue == null)
                {
                    // Write directly if is nto part of some queue bordered by scope
                    var group = new LogGroup();
                    group.Add(new LogGroup.Log(categoryName, severity, message, state, group.Depth, true));
                    var log = Formatter.FormatRequest(group, traceIdentifier);
                    if (!string.IsNullOrWhiteSpace(log))
                    {
                        _writer.WriteLog(log, group.Time.DateTime, traceIdentifier);
                    }
                    return;
                }

                queue.Add(new LogGroup.Log(categoryName, severity, message, state, queue.Depth, true));
            }
        }

        public void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state)
        {
            if (CanWrite(traceIdentifier))
            {
                var queue = LogGroups.GetQueue(traceIdentifier, true);
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
                    if (!string.IsNullOrWhiteSpace(log))
                    {
                        _writer.WriteLog(log, queue.Time.DateTime, traceIdentifier);
                    }
                }
                else{
                    //Write only increasing scopes and ignore decreasing scopes
                    var canWrite = CanWriteScope(state);
                    queue.Add(new LogGroup.Log(categoryName, LogLevel.None, "", state, depth, canWrite));
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
            foreach (var pair in LogGroups.GetAllQueues()){
                var log = Formatter.FormatRequest(pair.Value, pair.Key);
                if (!string.IsNullOrWhiteSpace(log)) _writer.WriteLog(log, pair.Value.Time.DateTime, pair.Key);
            }

            LogGroups.Dispose();
        }
    }
}