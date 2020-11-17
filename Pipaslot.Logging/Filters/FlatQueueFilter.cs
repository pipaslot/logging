using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Filters
{
    /// <summary>
    ///     Logging only for defined classes/scopes and their methods. Does not involve also deeper logging.
    /// </summary>
    internal class FlatQueueFilter : IQueueFilter
    {
        private readonly LogLevel _logLevel;

        public FlatQueueFilter(LogLevel logLevel)
        {
            _logLevel = logLevel;
        }

        public Queue Filter(Queue queue)
        {
            var records = new List<Record>(queue.Count);
            foreach (var log in queue)
            {
                if (log.Type == RecordType.Record && _logLevel <= log.Severity)
                {
                    records.Add(log);
                }
            }
            return queue.CloneWith(records);
        }

    }
}