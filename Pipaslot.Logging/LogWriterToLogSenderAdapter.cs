using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Logging.Records;

namespace Pipaslot.Logging
{
    /// <summary>
    /// Adapter providing Log sender with lazy loading
    /// </summary>
    /// <typeparam name="TLogSender"></typeparam>
    public class LogWriterToLogSenderAdapter<TLogSender> : ILogWriter where TLogSender : class, ILogSender
    {
        private readonly IServiceProvider _serviceProvider;

        public LogWriterToLogSenderAdapter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Write log scope to sender
        /// </summary>
        /// <param name="scope"></param>
        public void WriteLog(LogScope scope)
        {
            Task.Run(()=>
            {
                using var serviceScope = _serviceProvider.CreateScope();
                var sender = serviceScope.ServiceProvider.GetService<TLogSender>();
                sender.SendLog(scope).GetAwaiter().GetResult();
            });
        }
    }
}