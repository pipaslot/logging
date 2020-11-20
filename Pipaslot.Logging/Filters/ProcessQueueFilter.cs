using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Filters
{
    /// <summary>
    ///     LogRecord all processes together
    /// </summary>
    public class ProcessQueueFilter : IQueueFilter
    {

        public IQueue Filter(IQueue queue)
        {
            return queue.TraceIdentifier.StartsWith(Constants.CliTraceIdentifierPrefix)
                ? queue
                : queue.CloneEmpty();
        }
    }
}