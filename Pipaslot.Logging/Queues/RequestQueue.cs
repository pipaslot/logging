using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Records;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    ///     Logs all separated requests
    /// </summary>
    internal class RequestQueue : QueueBase
    {
        public RequestQueue(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanAddIntoExistingLogScope(string categoryName, LogLevel severity, LogScope scope)
        {
            return true;
        }

        protected override bool CanCreateNewLogScope(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return !traceIdentifier.StartsWith(Constants.CliTraceIdentifierPrefix);
        }
    }
}