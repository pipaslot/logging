using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Writers;

namespace Pipaslot.Logging
{
    public static class ILoggingBuilderExtensions
    {
        public static void AddRequestLogger(this ILoggingBuilder builder, LogLevel logLevel)
        {
            /*
            // Logging with own logger implementation
            services.AddLogger(LogLevel.Trace, s => new WriterCollection()
            {
                new RequestWriter(Path.Combine(Directory.GetCurrentDirectory(),"logs"), "{Date}-requests.log"),
                //new FlatWriter(logSettings.RootDirectory, logSettings.ApplicationName + "{Date}-service1.log", typeof(ServiceLevel1).FullName, Constants.Constant.Logging.Personalizations, LogLevel.Debug),
                //new FlatWriter(logSettings.RootDirectory, logSettings.ApplicationName + "{Date}-errors.log", LogLevel.Error),
                //new ProcessWriter(logSettings.RootDirectory, logSettings.ApplicationName + "{Date}-processes-{Id}.log"),
                //new SendWriter(s.GetService<EmailSender>(), LogLevel.Critical)
            });*/

            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddSingleton<IWriter>(s =>
            {
                return new RequestWriter(Path.Combine(Directory.GetCurrentDirectory(), "logs"),
                    "{Date}-requests.log");
            });
            builder.Services.AddSingleton<ILoggerProvider, LoggerProvider>(s =>
            {
                var provider = s.GetService<IHttpContextAccessor>();
                var writer = s.GetService<IWriter>();
                return new LoggerProvider(provider, writer, logLevel);
            });
        }
    }
}
