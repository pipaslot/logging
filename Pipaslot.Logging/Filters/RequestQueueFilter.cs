using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Filters
{
    /// <summary>
    ///     Logs all separated requests
    /// </summary>
    public class RequestQueueFilter : IQueueFilter
    {
        public Queue Filter(Queue queue)
        {
            return queue.TraceIdentifier.StartsWith(Constants.CliTraceIdentifierPrefix)
                ? queue.CloneEmpty()
                : queue;
        }
    }
}