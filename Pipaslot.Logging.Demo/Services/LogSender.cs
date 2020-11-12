using System;
using System.Threading.Tasks;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Demo.Services
{
    public class LogSender : ILogSender
    {
        private readonly QueueFormatter _formatter = new QueueFormatter();

        public Task SendLog(Queue queue)
        {
            var log = _formatter.Format(queue);
            if (!string.IsNullOrWhiteSpace(log))
            {
                Console.WriteLine(nameof(LogSender) + " - " + log);
            }
            return Task.CompletedTask;
        }
    }
}