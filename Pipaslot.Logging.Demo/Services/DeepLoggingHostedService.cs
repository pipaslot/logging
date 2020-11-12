using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Demo.Services
{
    public class DeepLoggingHostedService : IHostedService
    {
        private readonly ILogger<DeepLoggingHostedService> _logger;

        public DeepLoggingHostedService(ILogger<DeepLoggingHostedService> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using(_logger.BeginMethod(new {FakeData = "FakeValue"}))
            {
                _logger.LogInformation(nameof(DeepLoggingHostedService)+" - Hosted service started");
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                _logger.LogError(nameof(DeepLoggingHostedService)+" - Some error happened on hosted service");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(nameof(DeepLoggingHostedService)+" - Hosted service stopped");
            return Task.CompletedTask;
        }
    }
}