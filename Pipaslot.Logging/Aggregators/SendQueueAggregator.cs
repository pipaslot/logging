using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Aggregators
{
    internal class SendQueueAggregator : QueueAggregatorBase
    {
        private readonly LogLevel _minimalLogLevel;

        public SendQueueAggregator(IOptions<PipaslotLoggerOptions> options, LogLevel minimalLogLevel, ILogWriter writer) : base(options)
        {
            _minimalLogLevel = minimalLogLevel;
            Writer = writer;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanCreateNewLogScope(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return _minimalLogLevel <= severity;
        }

        protected override bool CanAddIntoExistingLogScope(string categoryName, LogLevel severity, Queue queue)
        {
            return true;
        }
    }
}