using System;

namespace Pipaslot.Logging
{
    public class LogWriterToLogSenderAdapter : ILogWriter
    {
        private readonly ILogSender _logSender;

        public LogWriterToLogSenderAdapter(ILogSender logSender)
        {
            _logSender = logSender;
        }

        public void WriteLog(string log, DateTime dateTime, string traceIdentifier)
        {
            _logSender.SendLog(log);
        }
    }
}