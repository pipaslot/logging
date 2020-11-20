using System.Collections.Generic;

namespace Pipaslot.Logging.Queues
{
    public static class IQueueExtensions
    {
        public static IQueue CloneWith(this IQueue queue, IEnumerable<Record> logs)
        {
            return new FixedSizeQueue(queue.TraceIdentifier, queue.Time, logs);
        }
        public static IQueue CloneEmpty(this IQueue queue)
        {
            return new EmptyQueue(queue.TraceIdentifier, queue.Time);
        }
    }
}