using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Groups;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Queues
{
    public class SendQueue : QueueBase
    {
        // private readonly LogGroupFormatter _formatter = new LogGroupFormatter();
        // private readonly LogGroupCollection _logGroups = new LogGroupCollection();
        private readonly LogLevel _logLevel;
        //private readonly ILogSender _logSender;

        public SendQueue(IOptions<PipaslotLoggerOptions> options, LogLevel logLevel, ILogSender logSender) : base(options)
        {
            _logLevel = logLevel;
            //_logSender = logSender;
            Writer = new LogWriterToLogSenderAdapter(logSender);
        }

        // public SendQueue(ILogSender logSender, LogLevel logLevel)
        // {
        //     _logSender = logSender;
        //     _logLevel = logLevel;
        // }
        //
        // public void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        // {
        //     //TODO Check log level
        //     var queue = _logGroups.GetQueue(traceIdentifier, true);
        //     if (queue == null){
        //         // Log should be ommited
        //         return;
        //     }
        //
        //     // Update depth
        //     var depth = queue.Depth;
        //     var stateType = typeof(TState);
        //     if (stateType == typeof(IncreaseScopeState))
        //         depth++;
        //     else if (stateType == typeof(DecreaseScopeState)) depth--;
        //     // Log or finish
        //     if (depth <= 0){
        //         if (queue.Logs.Any(l => (int) l.Severity >= (int) _logLevel)){
        //             var log = _formatter.FormatRequest(queue, traceIdentifier);
        //             _logSender.SendLog(log);
        //         }
        //
        //         // Remove request history from memory
        //         _logGroups.Remove(traceIdentifier);
        //     }
        //     else
        //         queue.Add(new LogGroup.Log(categoryName, severity, message, state, depth, true));
        // }
        //
        // public void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state)
        // {
        //     //TODO
        // }
        //
        // public virtual void Dispose()
        // {
        //     //write all remaining logs
        //     var sb = new StringBuilder();
        //     foreach (var pair in _logGroups.GetAllQueues()){
        //         if (pair.Value.Logs.Any(l => (int) l.Severity >= (int) _logLevel)){
        //             var log = _formatter.FormatRequest(pair.Value, pair.Key);
        //             sb.AppendLine(log);
        //         }
        //     }
        //
        //     Send(sb.ToString());
        //
        //     _logGroups.Dispose();
        // }

        // private void Send(string log)
        // {
        //     if (!string.IsNullOrWhiteSpace(log)){
        //         _logSender.SendLog(log);
        //     }
        // }

        protected override ILogWriter Writer { get; }
        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName, LogLevel severity, TState state)
        {
            return _logLevel <= severity;
        }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            return _logLevel <= severity;
        }
    }
}