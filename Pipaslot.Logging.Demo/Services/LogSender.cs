using System;

namespace Pipaslot.Logging.Demo.Services
{
    public class LogSender : ILogSender
    {
        public void SendLog(string log)
        {
            Console.WriteLine(nameof(LogSender) + " - " + log);
        }
    }
}