using System;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Queue aggregating log records with the same trace identifier
    /// </summary>
    public interface IQueue : IDisposable
    {
        /// <summary>
        /// Write message from logger
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="traceIdentifier">Request or process unique trace identifier</param>
        /// <param name="categoryName">Logger category name</param>
        /// <param name="severity">Message severity</param>
        /// <param name="message">Message</param>
        /// <param name="state">Custom state object</param>
        void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state);

        /// <summary>
        /// Write scope from logger
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="traceIdentifier">Request or process unique trace identifier</param>
        /// <param name="categoryName">Logger category name</param>
        /// <param name="state">Custom state object</param>
        void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state);
    }
}