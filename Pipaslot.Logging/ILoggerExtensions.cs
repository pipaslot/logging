using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging
{
    /// <summary>
    ///     Logger extension methods
    /// </summary>
    public static class LoggerExtensions
    {
        private static void LogInternal(this ILogger logger,
            LogLevel logLevel,
            string message,
            object data,
            Exception? exception)
        {
            logger.Log(logLevel, new EventId(0, message), data, exception, MessageFormatter);
        }

        private static string MessageFormatter(object state, Exception exception)
        {
            return state?.ToString() ?? "";
        }

        #region BeginMethod

        /// <summary>
        ///     Start logging action in separated scope. Caller class and method will be logged.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="state">Additional state data to be logged</param>
        /// <param name="memberName">Caller method name provided by compiler</param>
        /// <returns></returns>
        public static IDisposable BeginMethod(this ILogger logger, object? state = null, [CallerMemberName] string memberName = "")
        {
            var scopeState = new IncreaseMethodState(memberName, state);
            return logger.BeginScope(scopeState);
        }

        #endregion

        #region Trace

        /// <summary>
        ///     LogRecord Message with Trace severity containing additional data after message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">Plain message to be logged</param>
        /// <param name="data">Value or data structure for serialization into log file</param>
        public static void LogTraceWithData(this ILogger logger, string message, object data)
        {
            logger.LogInternal(LogLevel.Trace, message, data, null);
        }

        /// <summary>
        ///     LogRecord Message with Trace severity containing additional data after message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">Exception to be logged</param>
        /// <param name="message">Plain message to be logged</param>
        /// <param name="data">Value or data structure for serialization into log file</param>
        public static void LogTraceWithData(this ILogger logger, Exception exception, string message, object data)
        {
            logger.LogInternal(LogLevel.Trace, message, data, exception);
        }

        #endregion

        #region Debug

        /// <summary>
        ///     LogRecord Message with Debug severity containing additional data after message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">Plain message to be logged</param>
        /// <param name="data">Value or data structure for serialization into log file</param>
        public static void LogDebugWithData(this ILogger logger, string message, object data)
        {
            logger.LogInternal(LogLevel.Debug, message, data, null);
        }

        /// <summary>
        ///     LogRecord Message with Debug severity containing additional data after message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">Exception to be logged</param>
        /// <param name="message">Plain message to be logged</param>
        /// <param name="data">Value or data structure for serialization into log file</param>
        public static void LogDebugWithData(this ILogger logger, Exception exception, string message, object data)
        {
            logger.LogInternal(LogLevel.Debug, message, data, exception);
        }

        #endregion

        #region Information

        /// <summary>
        ///     LogRecord Message with Information severity containing additional data after message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">Plain message to be logged</param>
        /// <param name="data">Value or data structure for serialization into log file</param>
        public static void LogInformationWithData(this ILogger logger, string message, object data)
        {
            logger.LogInternal(LogLevel.Information, message, data, null);
        }

        /// <summary>
        ///     LogRecord Message with Information severity containing additional data after message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">Exception to be logged</param>
        /// <param name="message">Plain message to be logged</param>
        /// <param name="data">Value or data structure for serialization into log file</param>
        public static void LogInformationWithData(this ILogger logger, Exception exception, string message, object data)
        {
            logger.LogInternal(LogLevel.Information, message, data, exception);
        }

        #endregion

        #region Warning

        /// <summary>
        ///     LogRecord Message with Warning severity containing additional data after message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">Plain message to be logged</param>
        /// <param name="data">Value or data structure for serialization into log file</param>
        public static void LogWarningWithData(this ILogger logger, string message, object data)
        {
            logger.LogInternal(LogLevel.Warning, message, data, null);
        }

        /// <summary>
        ///     LogRecord Message with Warning severity containing additional data after message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">Exception to be logged</param>
        /// <param name="message">Plain message to be logged</param>
        /// <param name="data">Value or data structure for serialization into log file</param>
        public static void LogWarningWithData(this ILogger logger, Exception exception, string message, object data)
        {
            logger.LogInternal(LogLevel.Warning, message, data, exception);
        }

        #endregion

        #region Error

        /// <summary>
        ///     LogRecord Message with Error severity containing additional data after message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">Plain message to be logged</param>
        /// <param name="data">Value or data structure for serialization into log file</param>
        public static void LogErrorWithData(this ILogger logger, string message, object data)
        {
            logger.LogInternal(LogLevel.Error, message, data, null);
        }

        /// <summary>
        ///     LogRecord Message with Error severity containing additional data after message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">Exception to be logged</param>
        /// <param name="message">Plain message to be logged</param>
        /// <param name="data">Value or data structure for serialization into log file</param>
        public static void LogErrorWithData(this ILogger logger, Exception exception, string message, object data)
        {
            logger.LogInternal(LogLevel.Error, message, data, exception);
        }

        #endregion

        #region Critical

        /// <summary>
        ///     LogRecord Message with Critical severity containing additional data after message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">Plain message to be logged</param>
        /// <param name="data">Value or data structure for serialization into log file</param>
        public static void LogCriticalWithData(this ILogger logger, string message, object data)
        {
            logger.LogInternal(LogLevel.Critical, message, data, null);
        }

        /// <summary>
        ///     LogRecord Message with Critical severity containing additional data after message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">Exception to be logged</param>
        /// <param name="message">Plain message to be logged</param>
        /// <param name="data">Value or data structure for serialization into log file</param>
        public static void LogCriticalWithData(this ILogger logger, Exception exception, string message, object data)
        {
            logger.LogInternal(LogLevel.Critical, message, data, exception);
        }

        #endregion
    }
}