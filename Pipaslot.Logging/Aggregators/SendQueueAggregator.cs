using System.Linq;
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
            return true;
        }

        protected override bool CanAddIntoExistingLogScope(string categoryName, LogLevel severity, Queue queue)
        {
            return true;
        }

        protected override bool CanWriteQueueToOutput(Queue queue)
        {
            return queue.Logs.Any(log => log.Severity != LogLevel.None && _minimalLogLevel <= log.Severity);
        }
    }
}