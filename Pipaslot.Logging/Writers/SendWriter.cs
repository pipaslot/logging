using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.States;
using Pipaslot.Logging.Writers.Queues;

namespace Pipaslot.Logging.Writers
{
    public class SendWriter : IWriter
    {
        private readonly LoggedQueueCollection _queues = new LoggedQueueCollection();
        private readonly ILogSender _logSender;
        private readonly LogLevel _logLevel;
        private readonly QueueFormatter _formatter = new QueueFormatter();

        public SendWriter(ILogSender logSender, LogLevel logLevel)
        {
            this._logSender = logSender;
            _logLevel = logLevel;
        }

        public void Write<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            var queue = _queues.GetQueue(traceIdentifier, true);
            if (queue == null)
            {
                // Log should be ommited
                return;
            }
            // Update depth
            var depth = queue.Depth;
            var stateType = typeof(TState);
            if (stateType == typeof(IncreaseScopeState))
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
                if (queue.Logs.Any(l => (int)l.Severity >= (int)_logLevel))
                {
                    var log = _formatter.FormatRequest(queue, traceIdentifier, LogLevel.Trace);
                    _logSender.SendLog(log);
                }
                // Remove request history from memory
                _queues.Remove(traceIdentifier);
            }
            else
            {
                queue.Logs.Add(new LoggedQueue.Log(categoryName, severity, message, state, depth));
            }
        }

        public virtual void Dispose()
        {
            //write all remaining logs
            var sb = new StringBuilder();
            foreach (var pair in _queues.GetAllQueues())
            {
                if (pair.Value.Logs.Any(l => (int)l.Severity >= (int)_logLevel))
                {
                    var log = _formatter.FormatRequest(pair.Value, pair.Key, LogLevel.Trace);
                    sb.AppendLine(log);
                }
            }
            Send(sb.ToString());

            _queues.Dispose();
        }

        private void Send(string log)
        {
            if (!string.IsNullOrWhiteSpace(log))
            {
                _logSender.SendLog(log);
            }
        }
    }
}
