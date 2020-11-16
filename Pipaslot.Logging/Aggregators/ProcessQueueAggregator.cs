using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Aggregators
{
    /// <summary>
    ///     LogRecord all processes together
    /// </summary>
    internal class ProcessQueueAggregator : QueueAggregatorBase
    {
        public ProcessQueueAggregator(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
        }

        protected override ILogWriter Writer { get; }


        protected override Queue ProcessQueueBeforeWrite(Queue queue)
        {
            return queue.TraceIdentifier.StartsWith(Constants.CliTraceIdentifierPrefix)
                ? queue
                : queue.CloneEmpty();
        }
    }
}