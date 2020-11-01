using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    ///     Logging only for defined classes/scopes and their methods. Does not involve also deeper logging.
    /// </summary>
    public class TreeQueue : QueueBase
    {
        /// <summary>
        ///     Definition of classes and their methods to be tracked
        /// </summary>
        private readonly HashSet<string> _classes = new HashSet<string>();

        public TreeQueue(ILogWriter writer, IOptions<PipaslotLoggerOptions> options, params string[] namespaceOrClass)
            : this(writer, options)
        {
            var items = namespaceOrClass
                .Select(i => i.ToLower())
                .Distinct();
            foreach (var item in items){
                _classes.Add(item);
            }
        }

        public TreeQueue(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanWriteIntoExistingQueue(string categoryName, LogLevel severity)
        {
            return true;
        }

        protected override bool CanCreateNewQueue(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return Contains(categoryName);
        }

        private bool Contains(string categoryName)
        {
            var lowercase = categoryName.ToLower() ?? "";
            return _classes.Any(c => lowercase.StartsWith(c));
        }
    }
}