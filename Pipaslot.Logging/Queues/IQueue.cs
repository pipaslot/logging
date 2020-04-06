using System;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Queues
{
    public interface IQueue : IDisposable
    {
        void Write<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state);
    }
}
