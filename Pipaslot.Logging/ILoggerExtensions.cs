using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging
{
    public static class ILoggerExtensions
    {
        /// <summary>
        /// Start logging method action in separated scope. Logs caller class and method
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="state"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static IDisposable BeginMethod(this ILogger logger, object state = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            var scopeState = new IncreaseScopeState(memberName, state);
            return logger.BeginScope(scopeState);
        }

        /// <summary>
        /// Start logging method action in separated scope. Logs caller class and method
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="state"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static IDisposable BeginNamedMethod(this ILogger logger, string memberName, object state = null)
        {
            var scopeState = new IncreaseScopeState(memberName, state);
            return logger.BeginScope(scopeState);
        }

        public static void LogTraceWithData(this ILogger logger, string message, object data, Exception ex = null)
        {
            logger.Log<object>(LogLevel.Trace, new EventId(0, message), data, ex, MessageFormatter);
        }

        public static void LogInformationWithData(this ILogger logger, string message, object data, Exception ex = null)
        {
            logger.Log<object>(LogLevel.Information, new EventId(0, message), data, ex, MessageFormatter);
        }

        public static void LogWarningWithData(this ILogger logger, string message, object data, Exception ex = null)
        {
            logger.Log<object>(LogLevel.Warning, new EventId(0, message), data, ex, MessageFormatter);
        }

        public static void LogErrorWithData(this ILogger logger, string message, object data, Exception ex = null)
        {
            logger.Log<object>(LogLevel.Error, new EventId(0, message), data, ex, MessageFormatter);
        }

        public static void LogCriticalWithData(this ILogger logger, string message, object data, Exception ex = null)
        {
            logger.Log<object>(LogLevel.Critical, new EventId(0, message), data, ex, MessageFormatter);
        }

        private static string MessageFormatter(object state, Exception error)
        {
            return state?.ToString() ?? "";
        }
    }
}
