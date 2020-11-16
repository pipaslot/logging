using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Aggregators
{
    /// <summary>
    ///     Logging only for defined classes/scopes and their methods. Does not involve also deeper logging.
    /// </summary>
    internal class FlatQueueAggregator : QueueAggregatorBase
    {
        private readonly LogLevel _logLevel;

        public FlatQueueAggregator(ILogWriter writer, LogLevel logLevel, IOptions<PipaslotLoggerOptions> options) : base(writer, options)
        {
            _logLevel = logLevel;
        }
        
        protected override Queue ProcessQueueBeforeWrite(Queue queue)
        {
            var records = new List<Record>(queue.Count);
            int? endDepth = null;
            foreach (var log in queue)
            {
                if (log.Type == RecordType.Record)
                {
                    if (_logLevel <= log.Severity)
                    {
                        records.Add(log);
                        endDepth = log.Depth;
                    }
                }
                else
                {
                    if (endDepth != null && log.Depth >= endDepth.Value)
                    {
                        records.Add(log);
                    }
                }
            }
            return queue.CloneWith(records);
        }

    }
}