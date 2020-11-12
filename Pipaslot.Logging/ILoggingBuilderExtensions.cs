﻿using System.Linq;
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
        ///     LogRecord all messages grouped by HTTP requests into file
        /// </summary>
        public static void AddRequestLogger(this ILoggingBuilder builder, string fileSuffix = "-requests")
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueue>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var logWriterFactory = s.GetService<IFileWriterFactory>();
                var writer = logWriterFactory.Create("{Date}" + fileSuffix + ".log");
                return new RequestQueue(writer, options);
            });
        }

        /// <summary>
        ///     LogRecord single message with specified log level. Useful when you need separate only errors or critical failures
        /// </summary>
        public static void AddFlatLogger(this ILoggingBuilder builder, string fileSuffix, LogLevel logLevel)
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueue>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var logWriterFactory = s.GetService<IFileWriterFactory>();
                var writer = logWriterFactory.Create("{Date}" + fileSuffix + ".log");
                return new FlatQueue(writer, logLevel, options);
            });
        }

        /// <summary>
        ///     LogRecord single message with specified log level and class or methods. Useful when you need separate specific procedure
        /// </summary>
        public static void AddTreeLogger(this ILoggingBuilder builder, string fileSuffix, params string[] namespaceOrClass)
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueue>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var logWriterFactory = s.GetService<IFileWriterFactory>();
                var writer = logWriterFactory.Create("{Date}" + fileSuffix + ".log");
                return new TreeQueue(writer, options, namespaceOrClass);
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
            builder.Services.AddSingleton<IQueue>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var writer = s.GetService<LogWriterToLogSenderAdapter<TLogSender>>();
                return new SendQueue(options, logLevel, writer);
            });
        }

        /// <summary>
        ///     LogRecord writing all messages from every single process which is not handled as HTTP request. Useful for background jobs. Every thread will have own log file
        /// </summary>
        public static void AddProcessLogger(this ILoggingBuilder builder, string fileSuffix = "-processes")
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueue>(s =>
            {
                var options = s.GetService<IOptions<PipaslotLoggerOptions>>();
                var logWriterFactory = s.GetService<IFileWriterFactory>();
                var writer = logWriterFactory.Create("{Date}" + fileSuffix + ".log");
                return new ProcessQueue(writer, options);
            });
        }

        private static void AddPipaslotLoggerProvider(this ILoggingBuilder builder)
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.TryAddSingleton<IOptions<PipaslotLoggerOptions>, DefaultPipaslotLoggerOptions>();
            builder.Services.TryAddSingleton<IFileWriterFactory, FileWriterFactory>();
            if (builder.Services.All(s => s.ImplementationType != typeof(PipaslotLoggerProvider))) builder.Services.AddSingleton<ILoggerProvider, PipaslotLoggerProvider>();
        }
    }
}