using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Logs all separated requests
    /// </summary>
    public class RequestQueue : QueueBase
    {
        public RequestQueue(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName, 
            LogLevel severity, string message, TState state)
        {
            return true;
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName, LogLevel severity, TState state)
        {
            return !traceIdentifier.StartsWith(Constrants.CliTraceIdentifierPrefix);
        }
    }
}