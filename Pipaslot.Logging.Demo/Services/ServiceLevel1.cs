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

        public void LogScopeAndCriticalMessage()
        {
            using (_logger.BeginMethod()){
                _logger.LogCritical("Operation performed");
            }
        }

        public void LogMessage()
        {
            _logger.LogInformation("Operation performed");
            Delay();
        }

        public void LogMessage(int repeat)
        {
            for (int i = 0; i < repeat; i++){
                _logger.LogInformation("Operation performed");
            }

            Delay();
        }

        public void Noop()
        {
            Delay();
        }

        private void Delay()
        {
            for (int i = 0; i < 10000; i++){
                if (true) ;
            }
        }
    }
}