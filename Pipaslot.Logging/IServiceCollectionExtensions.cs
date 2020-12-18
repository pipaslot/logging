using System;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Logging.Configuration;

namespace Pipaslot.Logging
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Register hosted service removing old log files
        /// </summary>
        public static IServiceCollection AddLogFileEraseHostedService(this IServiceCollection services)
        {
            var options = new LogFileEraseHostedServiceOptions();
            return services.AddLogFileEraseHostedService(options);
        }

        /// <summary>
        /// Register hosted service removing old log files
        /// </summary>
        public static IServiceCollection AddLogFileEraseHostedService(this IServiceCollection services, Action<LogFileEraseHostedServiceOptions> setupOptions)
        {
            var options = new LogFileEraseHostedServiceOptions();
            setupOptions(options);
            return services.AddLogFileEraseHostedService(options);
        }

        private static IServiceCollection AddLogFileEraseHostedService(this IServiceCollection services, LogFileEraseHostedServiceOptions options)
        {
            services.AddSingleton<LogFileEraseHostedServiceOptions>(options);
            services.AddHostedService<LogFileEraseHostedService>();
            return services;
        }
    }
}
