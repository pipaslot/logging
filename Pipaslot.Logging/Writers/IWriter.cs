using System;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Writers
{
    public interface IWriter : IDisposable
    {
        void Write<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state);
    }
}
