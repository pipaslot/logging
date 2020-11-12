using System;
using System.Threading.Tasks;
using Pipaslot.Logging.Records;

namespace Pipaslot.Logging.Demo.Services
{
    public class LogSender : ILogSender
    {
        private readonly LogScopeFormatter _formatter = new LogScopeFormatter();

        public Task SendLog(LogScope scope)
        {
            var log = _formatter.Format(scope);
            if (!string.IsNullOrWhiteSpace(log))
            {
                Console.WriteLine(nameof(LogSender) + " - " + log);
            }
            return Task.CompletedTask;
        }
    }
}