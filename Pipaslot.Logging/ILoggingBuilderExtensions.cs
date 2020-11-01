using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging
{
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        ///     Log all messages grouped by HTTP requests into file
        /// </summary>
        public static void AddRequestLogger(this ILoggingBuilder builder, string fileSuffix = "-requests")
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueue>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var logWriterFactory = s.GetService<ILogWriterFactory>();
                var writer = logWriterFactory.Create("{Date}" + fileSuffix + ".log");
                return new RequestQueue(writer, options);
            });
        }

        /// <summary>
        ///     Log single message with specified log level. Useful when you need separate only errors or critical failures
        /// </summary>
        public static void AddFlatLogger(this ILoggingBuilder builder, string fileSuffix, LogLevel logLevel)
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueue>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var logWriterFactory = s.GetService<ILogWriterFactory>();
                var writer = logWriterFactory.Create("{Date}" + fileSuffix + ".log");
                return new FlatQueue(writer, logLevel, options);
            });
        }

        /// <summary>
        ///     Log single message with specified log level and class or methods. Useful when you need separate specific procedure
        /// </summary>
        public static void AddTreeLogger(this ILoggingBuilder builder, string fileSuffix, params string[] namespaceOrClass)
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueue>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var logWriterFactory = s.GetService<ILogWriterFactory>();
                var writer = logWriterFactory.Create("{Date}" + fileSuffix + ".log");
                return new TreeQueue(writer, options, namespaceOrClass);
            });
        }

        /// <summary>
        ///     Sends every message by provider log sender. Useful when you need send notifications about critical errors
        /// </summary>
        public static void AddSendMailLogger<TLogSender>(this ILoggingBuilder builder, LogLevel logLevel)
            where TLogSender : class, ILogSender
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.TryAddSingleton<TLogSender>();
            builder.Services.AddSingleton<IQueue>(s =>
            {
                var sender = s.GetService<TLogSender>();
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                return new SendQueue(options, logLevel, sender);
            });
        }

        /// <summary>
        ///     Log writing all messages from every single process which is not handled as HTTP request. Useful for background jobs. Every thread will have own log file
        /// </summary>
        public static void AddProcessLogger(this ILoggingBuilder builder, string fileSuffix = "-process-{Id}")
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueue>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var logWriterFactory = s.GetService<ILogWriterFactory>();
                var writer = logWriterFactory.Create("{Date}" + fileSuffix + ".log");
                return new ProcessQueue(writer, options);
            });
        }

        private static void AddPipaslotLoggerProvider(this ILoggingBuilder builder)
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.TryAddSingleton<IOptions<PipaslotLoggerOptions>, DefaultPipaslotLoggerOptions>();
            builder.Services.TryAddSingleton<ILogWriterFactory, LogWriterFactory>();
            if (builder.Services.All(s => s.ImplementationType != typeof(PipaslotLoggerProvider))) builder.Services.AddSingleton<ILoggerProvider, PipaslotLoggerProvider>();
        }
    }
}