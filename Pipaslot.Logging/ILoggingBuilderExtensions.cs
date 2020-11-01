using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        /// Log all messages grouped by HTTP requests into file
        /// </summary>
        public static void AddRequestLogger(this ILoggingBuilder builder, string fileSuffix = "-requests")
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueue>(s =>
                new RequestLogQueue(new FileLogWriter(s.GetService<IOptions<PipaslotLoggerOptions>>(), "{Date}" + fileSuffix + ".log"), s.GetService<IOptions<PipaslotLoggerOptions>>()));
        }

        /// <summary>
        /// Log single message with specified log level. Useful when you need separate only errors or critical failures
        /// </summary>
        public static void AddFlatLogger(this ILoggingBuilder builder, string fileSuffix, LogLevel logLevel)
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueue>(s =>
                new FlatActionCallLogQueue(new FileLogWriter(s.GetService<IOptions<PipaslotLoggerOptions>>(), "{Date}" + fileSuffix + ".log"), logLevel, s.GetService<IOptions<PipaslotLoggerOptions>>()));
        }

        /// <summary>
        /// Log single message with specified log level and class or methods. Useful when you need separate specific procedure
        /// </summary>
        public static void AddCallLogger(this ILoggingBuilder builder, string fileSuffix, LogLevel logLevel, string className)
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueue>(s =>
                new FlatActionCallLogQueue(new FileLogWriter(s.GetService<IOptions<PipaslotLoggerOptions>>(), "{Date}" + fileSuffix + ".log"), logLevel, s.GetService<IOptions<PipaslotLoggerOptions>>(), className));
        }

        /// <summary>
        /// Sends every message by provider log sender. Useful when you need send notifications about critical errors
        /// </summary>
        public static void AddSendMailLogger<TLogSender>(this ILoggingBuilder builder, LogLevel logLevel)
            where TLogSender : class, ILogSender
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.TryAddSingleton<TLogSender>();
            builder.Services.AddSingleton<IQueue>(s =>
            {
                var sender = s.GetService<TLogSender>();
                return new SendQueue(sender, logLevel);
            });
        }

        /// <summary>
        /// Log writing all messages from every single process which is not handled as HTTP request. Useful for background jobs. Every thread will have own log file
        /// </summary>
        public static void AddProcessLogger(this ILoggingBuilder builder, string fileSuffix = "-process-{Id}")
        {
            builder.AddPipaslotLoggerProvider();
            builder.Services.AddSingleton<IQueue>(s =>
                new ProcessQueue(new FileLogWriter(s.GetService<IOptions<PipaslotLoggerOptions>>(), "{Date}" + fileSuffix + ".log"), s.GetService<IOptions<PipaslotLoggerOptions>>()));
        }

        private static void AddPipaslotLoggerProvider(this ILoggingBuilder builder)
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.TryAddSingleton<IOptions<PipaslotLoggerOptions>, DefaultPipaslotLoggerOptions>();
            if (builder.Services.All(s => s.ImplementationType != typeof(PipaslotLoggerProvider)))
            {
                builder.Services.AddSingleton<ILoggerProvider, PipaslotLoggerProvider>();
            }
        }
    }
}