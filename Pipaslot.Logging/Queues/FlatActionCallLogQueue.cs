using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Logging only for defined classes/scopes and their methods. Does not involve also deeper logging.
    /// </summary>
    public class FlatActionCallLogQueue : QueueBase
    {
        /// <summary>
        /// Definition of classes and their methods to be tracked
        /// </summary>
        private readonly HashSet<string> _classesAndMethods = new HashSet<string>();

        private readonly LogLevel _logLevel;

        public FlatActionCallLogQueue(ILogWriter writer, LogLevel logLevel, IOptions<PipaslotLoggerOptions> options, string className)
            : this(writer, logLevel, options)
        {
            _classesAndMethods.Add(className);
        }

        public FlatActionCallLogQueue(ILogWriter writer, LogLevel logLevel, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
            _logLevel = logLevel;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName,
            LogLevel severity, string message, TState state)
        {
            if (_classesAndMethods.Count == 0){
                return true;
            }

            if (_classesAndMethods.Contains(categoryName)){
                return true;
            }

            return false;
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName,
            LogLevel severity, TState state)
        {
            if (_logLevel >= severity){
                if (_classesAndMethods.Count == 0){
                    return true;
                }

                var ics = state as IncreaseScopeState;
                if (_classesAndMethods.Contains(categoryName) && ics != null){
                    return true;
                }
            }

            return false;
        }
    }
}