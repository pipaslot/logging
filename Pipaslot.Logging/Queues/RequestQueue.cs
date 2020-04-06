using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Logs all separated requests
    /// </summary>
    public class RequestQueue : QueueBase
    {
        public RequestQueue(string path, string filename) : this(new WriterSetting(path, filename))
        {
        }
        
        public RequestQueue(WriterSetting setting) 
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
