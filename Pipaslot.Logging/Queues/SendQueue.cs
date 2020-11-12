using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pipaslot.Logging.Queues
{
    internal class SendQueue : QueueBase
    {
        private readonly LogLevel _logLevel;

        public SendQueue(IOptions<PipaslotLoggerOptions> options, LogLevel logLevel, ILogSender logSender) : base(options)
        {
            _logLevel = logLevel;
            Writer = new LogWriterToLogSenderAdapter(logSender);
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanCreateNewQueue(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return _logLevel <= severity;
        }

        protected override bool CanWriteIntoExistingQueue(string categoryName, LogLevel severity)
        {
            return true;
        }
    }
}