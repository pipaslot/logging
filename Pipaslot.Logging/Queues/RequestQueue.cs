using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        protected override bool CanWriteIntoExistingQueue(string categoryName, LogLevel severity)
        {
            return true;
        }

        protected override bool CanCreateNewQueue(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return !traceIdentifier.StartsWith(Constants.CliTraceIdentifierPrefix);
        }
    }
}