using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Start logging action in separated scope. Caller class and method will be logged.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="state">Additional state data to be logged</param>
        /// <param name="memberName">Caller method name provided by compiler</param>
        /// <returns></returns>
        public static IDisposable BeginMethod(this ILogger logger, object state = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            var scopeState = new IncreaseScopeState(memberName, state);
            return logger.BeginScope(scopeState);
        }
// TODO Implement
//        /// <summary>
//        /// Start invocation logging in separated scope specified by unique name.
//        /// </summary>
//        /// <see cref="Pipaslot.Logging.LoggingBuilderExtensions.AddCallLogger"/>
//        /// <seealso cref="DeepActionCallQueue"/>
//        /// <seealso cref="FlatActionCallQueue"/>
//        /// <param name="logger"></param>
//        /// <param name="uniqueIdentifier">Unique call identifier</param>
//        /// <param name="state">Additional state data to be logged</param>
//        /// <returns></returns>
//        public static IDisposable BeginAction(this ILogger logger, string uniqueIdentifier, object state = null)
//        {
//            var scopeState = new IncreaseActionScopeState(uniqueIdentifier, state);
//            return logger.BeginScope(scopeState);
//        }

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
