using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    ///     Log all processes together
    /// </summary>
    public class ProcessQueue : QueueBase
    {
        public ProcessQueue(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName, LogLevel severity, TState state)
        {
            return traceIdentifier?.StartsWith(Constrants.CliTraceIdentifierPrefix) ?? false;
        }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            return true;
        }
    }
}