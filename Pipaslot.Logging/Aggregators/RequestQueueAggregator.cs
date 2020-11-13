using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Aggregators
{
    /// <summary>
    ///     Logs all separated requests
    /// </summary>
    internal class RequestQueueAggregator : QueueAggregatorBase
    {
        public RequestQueueAggregator(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanAddIntoExistingLogScope(string categoryName, LogLevel severity, Queue queue)
        {
            return true;
        }

        protected override bool CanCreateNewLogScope(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return !traceIdentifier.StartsWith(Constants.CliTraceIdentifierPrefix);
        }
        
        protected override bool CanWriteQueueToOutput(Queue queue)
        {
            return true;
        }
    }
}