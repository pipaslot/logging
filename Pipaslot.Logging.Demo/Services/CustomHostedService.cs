using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Demo.Services
{
    public class CustomHostedService : IHostedService
    {
        private readonly ILogger<CustomHostedService> _logger;

        public CustomHostedService(ILogger<CustomHostedService> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hosted service started");
            await Task.Delay(TimeSpan.FromSeconds(1));
            _logger.LogError("Some error happened on hosted service");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hosted service stopped");
            return Task.CompletedTask;
        }
    }
}