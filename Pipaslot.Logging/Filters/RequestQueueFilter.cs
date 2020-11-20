using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Filters
{
    /// <summary>
    ///     Logs all separated requests
    /// </summary>
    public class RequestQueueFilter : IQueueFilter
    {
        public IQueue Filter(IQueue queue)
        {
            return queue.TraceIdentifier.StartsWith(Constants.CliTraceIdentifierPrefix)
                ? queue.CloneEmpty()
                : queue;
        }
    }
}