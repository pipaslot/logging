using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Demo.Services
{
    
    public class FlatLoggingHostedService : IHostedService
    {
        private readonly ILogger<FlatLoggingHostedService> _logger;

        public FlatLoggingHostedService(ILogger<FlatLoggingHostedService> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(nameof(FlatLoggingHostedService)+" - Hosted service started");
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            _logger.LogError(nameof(FlatLoggingHostedService)+" - Some error happened on hosted service");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(nameof(FlatLoggingHostedService)+" - Hosted service stopped");
            return Task.CompletedTask;
        }
    }
}