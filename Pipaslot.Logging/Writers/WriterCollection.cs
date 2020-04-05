using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Writers
{
    public class WriterCollection : List<IWriter>, IWriter
    {
        public void Write<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            foreach (var writer in this)
            {
                writer.Write(traceIdentifier, categoryName, severity, message, state);
            }
        }

        public void Dispose()
        {
            foreach (var writer in this)
            {
                writer.Dispose();
            }
        }
    }
}
