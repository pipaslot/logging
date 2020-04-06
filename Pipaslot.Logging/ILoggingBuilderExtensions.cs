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
        /// <summary>
        /// Log all messages grouped by HTTP requests into file
        /// </summary>
        public static void AddRequestLogger(this ILoggingBuilder builder, string directory, LogLevel logLevel, string fileSuffix = "-requests")
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.TryAddSingleton<ILoggerProvider, LoggerProvider>();
            builder.Services.AddSingleton<IWriter>(s => new RequestWriter(directory, "{Date}" + fileSuffix + ".log"));
        }

        /// <summary>
        /// Log single message with specified log level. Useful when you need separate only errors or critical failures
        /// </summary>
        public static void AddFlatLogger(this ILoggingBuilder builder, string directory, string fileSuffix, LogLevel logLevel)
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.TryAddSingleton<ILoggerProvider, LoggerProvider>();
            builder.Services.AddSingleton<IWriter>(s => new FlatWriter(directory, "{Date}" + fileSuffix + ".log"));
        }
        
        /// <summary>
        /// Log single message with specified log level and class or methods. Useful when you need separate specific procedure
        /// </summary>
        public static void AddCallLogger(this ILoggingBuilder builder, string directory, string fileSuffix, LogLevel logLevel, string className, params string[] methodNames)
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.TryAddSingleton<ILoggerProvider, LoggerProvider>();
            builder.Services.AddSingleton<IWriter>(s => new FlatWriter(directory, "{Date}" + fileSuffix + ".log", className, methodNames));
        }

        /// <summary>
        /// Sends every message by provider log sender. Useful when you need send notifications about critical errors
        /// </summary>
        public static void AddSendMailLogger<TLogSender>(this ILoggingBuilder builder, LogLevel logLevel)
        where TLogSender : class, ILogSender
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.TryAddSingleton<ILoggerProvider, LoggerProvider>();
            builder.Services.TryAddSingleton<TLogSender>();
            builder.Services.AddSingleton<IWriter>(s =>
            {
                var sender = s.GetService<TLogSender>();
                return new SendWriter(sender, logLevel);
            });
        }

        /// <summary>
        /// Log writing all messages from every single process which is not handled as HTTP request. Useful for background jobs.
        /// </summary>
        public static void AddProcessLogger(this ILoggingBuilder builder, string directory, LogLevel logLevel, string fileSuffix = "-process-{Id}")
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.TryAddSingleton<ILoggerProvider, LoggerProvider>();
            builder.Services.AddSingleton<IWriter>(s => new ProcessWriter(directory, "{Date}" + fileSuffix + ".log"));
        }
    }
}
