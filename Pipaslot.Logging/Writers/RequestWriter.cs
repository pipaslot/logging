using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Writers
{
    /// <summary>
    /// Logs all separated requests
    /// </summary>
    public class RequestWriter : WriterBase
    {
        public RequestWriter(string path, string filename) : this(new WriterSetting(path, filename))
        {
        }
        
        public RequestWriter(WriterSetting setting) 
        {
            Writer = new FileLogWriter(setting);
            LogLevel = setting.LogLevel;
        }

        protected override ILogWriter Writer { get; }
        protected override LogLevel LogLevel { get; }

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
