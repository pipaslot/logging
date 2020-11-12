using System;
using System.Collections.Generic;
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

        public void WriteLog(string logContent, DateTime dateTime, string traceIdentifier, IReadOnlyCollection<LogRecord> logRecords)
        {
            Task.Run(()=>
            {
                _logSender.SendLog(logContent, logRecords).GetAwaiter().GetResult();
            });
        }
    }
}