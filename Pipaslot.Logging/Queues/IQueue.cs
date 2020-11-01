using System;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Queues
{
    public interface IQueue : IDisposable
    {
        void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state);
        void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state);
    }
}
