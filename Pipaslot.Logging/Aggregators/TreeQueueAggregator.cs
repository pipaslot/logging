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
    internal class TreeQueueAggregator : QueueAggregatorBase
    {
        /// <summary>
        ///     Definition of classes and their methods to be tracked
        /// </summary>
        private readonly HashSet<string> _classes = new HashSet<string>();

        public TreeQueueAggregator(ILogWriter writer, IOptions<PipaslotLoggerOptions> options, params string[] namespaceOrClass)
            : this(writer, options)
        {
            var items = namespaceOrClass
                .Select(i => i.ToLower())
                .Distinct();
            foreach (var item in items)
            {
                _classes.Add(item);
            }
        }

        public TreeQueueAggregator(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(writer, options)
        {
        }

        protected override Queue ProcessQueueBeforeWrite(Queue queue)
        {
            var records = new List<Record>(queue.Logs.Count);
            int? endDepth = null;
            foreach (var log in queue.Logs)
            {
                var lowercase = log.CategoryName.ToLower() ?? "";
                var isIncluded = _classes.Any(c => lowercase.StartsWith(c));
                if (endDepth == null && isIncluded)
                {
                    endDepth = log.Depth;
                }
                if (endDepth != null && log.Depth >= endDepth.Value)
                {
                    records.Add(log);
                }
                else if (endDepth != null && log.Depth == endDepth.Value - 1 && log.Type == RecordType.ScopeEndIgnored)
                {
                    records.Add(log);
                }
            }
            return queue.CloneWith(records);
        }
    }
}