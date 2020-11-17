using System.Collections.Generic;
using System.Linq;
using Pipaslot.Logging.Filters;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    ///     Logging only for defined classes/scopes and their methods. Does not involve also deeper logging.
    /// </summary>
    internal class TreeQueueFilter : IQueueFilter
    {
        /// <summary>
        ///     Definition of classes and their methods to be tracked
        /// </summary>
        private readonly HashSet<string> _classes = new HashSet<string>();

        public TreeQueueFilter(params string[] namespaceOrClass)
        {
            var items = namespaceOrClass
                .Select(i => i.ToLower())
                .Distinct();
            foreach (var item in items)
            {
                _classes.Add(item);
            }
        }
        
        public Queue Filter(Queue queue)
        {
            var records = new List<Record>(queue.Count);
            int? endDepth = null;
            foreach (var log in queue)
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