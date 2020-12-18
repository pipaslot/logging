using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pipaslot.Logging.Configuration;

namespace Pipaslot.Logging
{
    public class LogFileEraseHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public LogFileEraseHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested){
                TimeSpan repeatInterval;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var options = scope.ServiceProvider.GetRequiredService<LogFileEraseHostedServiceOptions>();
                    repeatInterval = options.RepeatInterval;
                    var eraser = scope.ServiceProvider.GetRequiredService<FileEraser>();
                    eraser.Run(options.MaxAge);
                }
                await Task.Delay(repeatInterval, stoppingToken);
            }
        }
    }
}