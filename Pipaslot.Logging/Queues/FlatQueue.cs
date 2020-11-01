using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    ///     Logging only for defined classes/scopes and their methods. Does not involve also deeper logging.
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

        protected override bool CanWriteIntoExistingQueue(string categoryName, LogLevel severity)
        {
            return _logLevel <= severity;
        }

        protected override bool CanCreateNewQueue(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return _logLevel <= severity;
        }
    }
}