using Pipaslot.Logging.Filters;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.Writers;

namespace Pipaslot.Logging
{
    /// <summary>
    ///     Pipe filtering and processing aggregated queues
    /// </summary>
    public class Pipe
    {
        private readonly IQueueFilter _filter;
        private readonly ILogWriter _writer;

        public Pipe(ILogWriter writer, IQueueFilter filter)
        {
            _writer = writer;
            _filter = filter;
        }
        
        public void Process(IQueue queue)
        {
            var processed = _filter.Filter(queue);
            if (processed.HasAnyRecord()) _writer.WriteLog(processed);
        }
    }
}