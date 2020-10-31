using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Logging only for defined classes/scopes and their methods. Does not involve also deeper logging.
    /// </summary>
    public class FlatActionCallQueue : QueueBase
    {
        /// <summary>
        /// Definition of classes and their methods to be tracked
        /// </summary>
        private readonly HashSet<string> _classesAndMethods = new HashSet<string>();

        public FlatActionCallQueue(ILogWriter writer, LogLevel logLevel, string className)
            : this(writer, logLevel)
        {
            _classesAndMethods.Add(className);
        }

        public FlatActionCallQueue(ILogWriter writer, LogLevel logLevel)
        {
            Writer = writer;
            LogLevel = logLevel;
        }

        protected override ILogWriter Writer { get; }
        protected override LogLevel LogLevel { get; }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName, 
            LogLevel severity, string message, TState state)
        {
            //TODO Check log level
            if (_classesAndMethods.Count == 0){
                return true;
            }

            if (_classesAndMethods.Contains(categoryName)){
                return true;
            }

            return false;
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName,
            LogLevel severity, string message, TState state)
        {
            //TODO Check log level
            if (_classesAndMethods.Count == 0){
                return true;
            }

            var ics = state as IncreaseScopeState;
            if (_classesAndMethods.Contains(categoryName) && ics != null){
                return true;
            }

            return false;
        }
    }
}