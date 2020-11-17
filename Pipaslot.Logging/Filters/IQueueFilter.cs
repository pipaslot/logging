using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Filters
{
    /// <summary>
    /// Filter records written into queue and clone with filtered records if some of them should be ignored
    /// </summary>
    public interface IQueueFilter
    {
        Queue Filter(Queue queue);
    }
}