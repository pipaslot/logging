using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Filters;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging
{
    /// <summary>
    ///     Extensions registering Provider with specified queue aggregators
    /// </summary>
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        ///     LogRecord all messages grouped by HTTP requests into file
        /// </summary>
        public static void AddRequestLogger(this ILoggingBuilder builder, string name = "requests", RollingInterval rollingInterval = RollingInterval.Day)
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueueAggregator>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var logWriterFactory = s.GetService<IFileWriterFactory>();
                var writer = logWriterFactory.Create(name, rollingInterval);
                return new QueueAggregator(writer, options, new RequestQueueFilter());
            });
        }

        /// <summary>
        ///     LogRecord single message with specified log level. Useful when you need separate only errors or critical failures
        /// </summary>
        public static void AddFlatLogger(this ILoggingBuilder builder, string name, LogLevel logLevel, RollingInterval rollingInterval = RollingInterval.Day)
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueueAggregator>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var logWriterFactory = s.GetService<IFileWriterFactory>();
                var writer = logWriterFactory.Create(name, rollingInterval);
                return new QueueAggregator(writer, options, new FlatQueueFilter(logLevel));
            });
        }


        /// <summary>
        ///     LogRecord single message with specified log level and class or methods. Useful when you need separate specific procedure
        /// </summary>
        public static void AddTreeLogger(this ILoggingBuilder builder, string name, params string[] namespaceOrClass)
        {
            builder.AddTreeLogger(name, RollingInterval.Day, namespaceOrClass);
        }

        /// <summary>
        ///     LogRecord single message with specified log level and class or methods. Useful when you need separate specific procedure
        /// </summary>
        public static void AddTreeLogger(this ILoggingBuilder builder, string name, RollingInterval rollingInterval, params string[] namespaceOrClass)
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueueAggregator>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var logWriterFactory = s.GetService<IFileWriterFactory>();
                var writer = logWriterFactory.Create(name, rollingInterval);
                return new QueueAggregator(writer, options, new TreeQueueFilter(namespaceOrClass));
            });
        }

        /// <summary>
        ///     Sends every message by provider log sender. Useful when you need send notifications about critical errors
        /// </summary>
        public static void AddSendLogger<TLogSender>(this ILoggingBuilder builder, LogLevel logLevel)
            where TLogSender : class, ILogSender
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.TryAddScoped<TLogSender>();
            builder.Services.TryAddSingleton<LogWriterToLogSenderAdapter<TLogSender>>();
            builder.Services.AddSingleton<IQueueAggregator>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var writer = s.GetService<LogWriterToLogSenderAdapter<TLogSender>>();
                return new QueueAggregator(writer, options, new SendQueueFilter(logLevel));
            });
        }

        /// <summary>
        ///     LogRecord writing all messages from every single process which is not handled as HTTP request. Useful for background jobs. Every thread will have own log file
        /// </summary>
        public static void AddProcessLogger(this ILoggingBuilder builder, string name = "processes", RollingInterval rollingInterval = RollingInterval.Day)
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueueAggregator>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var logWriterFactory = s.GetService<IFileWriterFactory>();
                var writer = logWriterFactory.Create(name, rollingInterval);
                return new QueueAggregator(writer, options, new ProcessQueueFilter());
            });
        }

        private static void AddPipaslotLoggerProvider(this ILoggingBuilder builder)
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.TryAddSingleton<IOptions<PipaslotLoggerOptions>, DefaultPipaslotLoggerOptions>();
            builder.Services.TryAddSingleton<IFileWriterFactory, FileWriterFactory>();
            builder.Services.TryAddSingleton<IFileNameFormatter, DefaultFileNameFormatter>();
            builder.Services.TryAddTransient<FileEraser>();
            if (builder.Services.All(s => s.ImplementationType != typeof(PipaslotLoggerProvider))) builder.Services.AddSingleton<ILoggerProvider, PipaslotLoggerProvider>();
        }
    }
}