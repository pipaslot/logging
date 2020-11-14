using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Demo.Services
{
    public class LogFileEraseHostedService : BackgroundService
    {
        private readonly ILogger<LogFileEraseHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _maxAge = TimeSpan.FromDays(1);
        private readonly TimeSpan _repeatInterval = TimeSpan.FromDays(1);

        public LogFileEraseHostedService(ILogger<LogFileEraseHostedService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested){
                using (var scope = _serviceProvider.CreateScope()){
                    var eraser = scope.ServiceProvider.GetService<FileEraser>();
                    var erasedCount = eraser.Run(_maxAge);
                    _logger.LogInformation("Erased {0} log files", erasedCount);
                }
                await Task.Delay(_repeatInterval, stoppingToken);
            }
        }
    }
}