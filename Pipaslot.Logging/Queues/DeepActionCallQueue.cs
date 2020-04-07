using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Logging for defined classes/scopes and their methods. Involves also all deeper logging
    /// </summary>
    public class DeepActionCallQueue : QueueBase
    {
        /// <summary>
        /// Definition of classes and their methods to be tracked
        /// </summary>
        private readonly Dictionary<string, string[]> _classesAndMethods = new Dictionary<string, string[]>();

        public DeepActionCallQueue(ILogWriter writer, LogLevel logLevel, Dictionary<string, string[]> classesAndMethods)
            : this(writer, logLevel)
        {
            _classesAndMethods = classesAndMethods ?? new Dictionary<string, string[]>();
        }


        public DeepActionCallQueue(ILogWriter writer, LogLevel logLevel, string className, params string[] methodNames)
            : this(writer, logLevel)
        {
            _classesAndMethods.Add(className, methodNames ?? new string[0]);
        }

        public DeepActionCallQueue(ILogWriter writer, LogLevel logLevel)
        {
            Writer = writer;
            LogLevel = logLevel;
        }

        protected override ILogWriter Writer { get; }
        protected override LogLevel LogLevel { get; }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName, string memberName,
            LogLevel severity, string message, TState state)
        {
            return true; //TODO Check log level
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName,
            LogLevel severity, string message, TState state)
        {
            //TODO Check log level
            var ics = state as IncreaseScopeState;
            if (_classesAndMethods.ContainsKey(categoryName) && ics != null)
            {
                var methodNames = _classesAndMethods[categoryName];
                if (methodNames.Length == 0 || methodNames.Contains(ics.CallerMemberName))
                {
                    return true;
                }
            }

            return false;
        }
    }
}