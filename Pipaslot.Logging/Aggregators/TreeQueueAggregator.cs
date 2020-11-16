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

        public TreeQueueAggregator(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanCreateNewLogScope(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return Contains(categoryName);
        }

        private bool Contains(string categoryName)
        {
            var lowercase = categoryName.ToLower() ?? "";
            return _classes.Any(c => lowercase.StartsWith(c));
        }
        protected override Queue ProcessQueueBeforeWrite(Queue queue)
        {
            return queue;
        }
    }
}