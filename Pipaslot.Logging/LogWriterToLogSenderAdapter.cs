using System.Threading.Tasks;
using Pipaslot.Logging.Records;

namespace Pipaslot.Logging
{
    public class LogWriterToLogSenderAdapter : ILogWriter
    {
        private readonly ILogSender _logSender;

        public LogWriterToLogSenderAdapter(ILogSender logSender)
        {
            _logSender = logSender;
        }

        public void WriteLog(LogScope scope)
        {
            Task.Run(()=>
            {
                _logSender.SendLog(scope).GetAwaiter().GetResult();
            });
        }
    }
}