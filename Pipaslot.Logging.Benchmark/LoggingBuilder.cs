using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Benchmark
{
    public class LoggingBuilder : ILoggingBuilder
    {
        public LoggingBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}