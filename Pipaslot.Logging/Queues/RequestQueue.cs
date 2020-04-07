using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Logs all separated requests
    /// </summary>
    public class RequestQueue : QueueBase
    {
        public RequestQueue(ILogWriter writer, LogLevel logLevel)
        {
            Writer = writer;
            LogLevel = logLevel;
        }

        protected override ILogWriter Writer { get; }
        protected override LogLevel LogLevel { get; }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName, string memberName,
            LogLevel severity, string message, TState state)
        {
            //TODO Check log level
            return true;
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName,
            LogLevel severity, string message, TState state)
        {
            //TODO Check log level
            return categoryName == "Microsoft.AspNetCore.Hosting.Internal.WebHost";
        }
    }
}