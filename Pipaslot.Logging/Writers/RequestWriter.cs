using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Writers
{
    /// <summary>
    /// Loggs all separated requests
    /// </summary>
    public class RequestWriter : WriterBase
    {
        public RequestWriter(WriterSetting setting) : base(setting)
        {
        }

        public RequestWriter(string path, string filename) : base(new WriterSetting(path, filename))
        {
        }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName, string memberName, LogLevel severity, string message, TState state)
        {
            return true;
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            return categoryName == "Microsoft.AspNetCore.Hosting.Internal.WebHost";
        }
    }
}
