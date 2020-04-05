using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Writers;

namespace Pipaslot.Logging
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddLogger(this IServiceCollection services, LogLevel level, Func<IServiceProvider, IWriter> implementationFactory)
        {
            services.AddSingleton<IWriter>(implementationFactory);
            services.AddSingleton<LoggerProvider>(s =>
            {
                var provider = s.GetService<IHttpContextAccessor>();
                var writer = s.GetService<IWriter>();
                return new LoggerProvider(provider, writer, level);
            });
            
            return services;
        }
    }
}
