using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Records;

namespace Pipaslot.Logging.Queues
{
    internal class SendQueue : QueueBase
    {
        private readonly LogLevel _minimalLogLevel;

        public SendQueue(IOptions<PipaslotLoggerOptions> options, LogLevel minimalLogLevel, ILogWriter writer) : base(options)
        {
            _minimalLogLevel = minimalLogLevel;
            Writer = writer;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanCreateNewLogScope(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return _minimalLogLevel <= severity;
        }

        protected override bool CanAddIntoExistingLogScope(string categoryName, LogLevel severity, LogScope scope)
        {
            return true;
        }
    }
}