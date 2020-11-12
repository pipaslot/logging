using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Records;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    ///     Logging only for defined classes/scopes and their methods. Does not involve also deeper logging.
    /// </summary>
    internal class FlatQueue : QueueBase
    {
        private readonly LogLevel _logLevel;

        public FlatQueue(ILogWriter writer, LogLevel logLevel, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
            _logLevel = logLevel;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanCreateNewLogScope(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return _logLevel <= severity;
        }

        protected override bool CanAddIntoExistingLogScope(string categoryName, LogLevel severity, LogScope scope)
        {
            return _logLevel <= severity;
        }
    }
}