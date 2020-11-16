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

        protected override bool CanAddIntoExistingLogScope(string traceIdentifier, string categoryName, LogLevel severity, Queue queue)
        {
            return true;
        }

        protected override Queue ProcessQueueBeforeWrite(Queue queue)
        {
            // Filter all records with severity lower than minimal
            var logs = queue.Logs.Where(log =>
                log.Type != RecordType.Record // Keep all scope records
                || (log.Type == RecordType.Record && log.Severity != LogLevel.None && _minimalLogLevel <= log.Severity));
            return queue.CloneWith(logs);
        }
    }
}