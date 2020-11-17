using System.Linq;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Filters
{
    public class SendQueueFilter : IQueueFilter
    {
        private readonly LogLevel _minimalLogLevel;

        public SendQueueFilter( LogLevel minimalLogLevel)
        {
            _minimalLogLevel = minimalLogLevel;
        }


        public Queue Filter(Queue queue)
       {
           return queue.Any(log =>
               log.Type == RecordType.Record && _minimalLogLevel <= log.Severity && log.Severity != LogLevel.None)
               ? queue
               : queue.CloneEmpty();
        }
    }
}