using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Demo.Services
{
    public class ServiceLevel1
    {
        private readonly ILogger<ServiceLevel1> _logger;

        public ServiceLevel1(ILogger<ServiceLevel1> logger)
        {
            _logger = logger;
        }

        public void PerformOperationWithLogging()
        {
            using (_logger.BeginMethod()){
                _logger.LogCritical("Operation performed");
            }
        }

        public void PerformOperationWithoutLogging()
        {
            //noop
        }
    }
}