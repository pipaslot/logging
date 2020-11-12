using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Aggregators
{
    /// <summary>
    ///     Logging only for defined classes/scopes and their methods. Does not involve also deeper logging.
    /// </summary>
    internal class FlatQueueAggregator : QueueAggregatorBase
    {
        private readonly LogLevel _logLevel;

        public FlatQueueAggregator(ILogWriter writer, LogLevel logLevel, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
            _logLevel = logLevel;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanCreateNewLogScope(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return _logLevel <= severity;
        }

        protected override bool CanAddIntoExistingLogScope(string categoryName, LogLevel severity, Queue queue)
        {
            return _logLevel <= severity;
        }
    }
}