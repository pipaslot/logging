using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pipaslot.Logging.Records;

namespace Pipaslot.Logging.Demo.Services
{
    public class LogSender : ILogSender
    {
        public Task SendLog(string log, IReadOnlyCollection<LogRecord> logRecords)
        {
            Console.WriteLine(nameof(LogSender) + " - " + log);
            return Task.CompletedTask;
        }
    }
}