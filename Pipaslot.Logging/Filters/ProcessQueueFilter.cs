using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Filters
{
    /// <summary>
    ///     LogRecord all processes together
    /// </summary>
    internal class ProcessQueueFilter : IQueueFilter
    {

        public Queue Filter(Queue queue)
        {
            return queue.TraceIdentifier.StartsWith(Constants.CliTraceIdentifierPrefix)
                ? queue
                : queue.CloneEmpty();
        }
    }
}