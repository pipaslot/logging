using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Logging only for defined classes/scopes and their methods. Does not involve also deeper logging.
    /// </summary>
    public class FlatQueue : QueueBase
    {
        private readonly LogLevel _logLevel;

        public FlatQueue(ILogWriter writer, LogLevel logLevel, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
            _logLevel = logLevel;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName,
            LogLevel severity, string message, TState state)
        {
            return _logLevel <= severity;
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName,
            LogLevel severity, TState state)
        {
            return _logLevel <= severity;
        }
    }
}