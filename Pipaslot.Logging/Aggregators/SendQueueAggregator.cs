using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Aggregators
{
    internal class SendQueueAggregator : QueueAggregatorBase
    {
        private readonly LogLevel _minimalLogLevel;

        public SendQueueAggregator(IOptions<PipaslotLoggerOptions> options, LogLevel minimalLogLevel, ILogWriter writer) : base(writer, options)
        {
            _minimalLogLevel = minimalLogLevel;
        }


       protected override Queue ProcessQueueBeforeWrite(Queue queue)
       {
           return queue.Logs.Any(log =>
               log.Type == RecordType.Record && _minimalLogLevel <= log.Severity && log.Severity != LogLevel.None)
               ? queue
               : queue.CloneEmpty();
        }
    }
}