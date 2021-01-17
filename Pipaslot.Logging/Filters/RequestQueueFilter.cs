using System.Linq;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Filters
{
    /// <summary>
    ///     Logs all separated requests
    /// </summary>
    public class RequestQueueFilter : IQueueFilter
    {
        private readonly LogLevel _minimalLogLevel;

        public RequestQueueFilter(LogLevel minimalLogLevel)
        {
            this._minimalLogLevel = minimalLogLevel;
        }

        public IQueue Filter(IQueue queue)
        {
            return queue.TraceIdentifier.StartsWith(Constants.CliTraceIdentifierPrefix)
                   || queue.All(t => t.Severity < _minimalLogLevel)
                ? queue.CloneEmpty()
                : queue;
        }
    }
}