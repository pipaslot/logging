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

        public TreeQueue(ILogWriter writer, IOptions<PipaslotLoggerOptions> options, string className)
            : this(writer, options)
        {
            _classes.Add(className.ToLower());
        }

        public TreeQueue(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName,
            LogLevel severity, string message, TState state)
        {
            return Contains(categoryName);
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName,
            LogLevel severity, TState state)
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