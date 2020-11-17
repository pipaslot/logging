using Pipaslot.Logging.Filters;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging
{
    public class Pipe
    {
        private readonly ILogWriter _writer;
        private readonly IQueueFilter _filter;

        public Pipe(ILogWriter writer, IQueueFilter filter)
        {
            _writer = writer;
            _filter = filter;
        }

        public void Process(Queue queue)
        {
            var processed = _filter.Filter(queue);
            if(processed.HasAnyRecord()){
                _writer.WriteLog(processed);
            }
        }
    }
}
