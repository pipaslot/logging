using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.WebApp.Services
{
    public class ServiceLevel1
    {
        private readonly ILogger<ServiceLevel1> _logger;

        public ServiceLevel1(ILogger<ServiceLevel1> logger)
        {
            _logger = logger;
        }

        public void PerformOperation()
        {
            using (_logger.BeginMethod())
            {
                _logger.LogInformation("Operation performed");
            }
        }
    }
}
