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
    public class FlatQueue : QueueBase
    {
        
        /// <summary>
        /// Definition of classes and their methods to be tracked
        /// </summary>
        private readonly Dictionary<string, string[]> _classesAndMethods = new Dictionary<string, string[]>();
        
        public FlatQueue(WriterSetting setting, string className, params string[] methodNames) : this(setting)
        {
            _classesAndMethods.Add(className, methodNames.ToArray());
        }
        
        public FlatQueue(string path, string filename, string className, string[] methodNames, LogLevel logLevel = LogLevel.Information) : this(new WriterSetting(path, filename, logLevel))
        {
            _classesAndMethods.Add(className, methodNames.ToArray());
        }

        public FlatQueue(string path, string filename, LogLevel logLevel = LogLevel.Information) : this(new WriterSetting(path, filename, logLevel))
        {
        }
        public FlatQueue(WriterSetting setting)
        {
            Writer = new FileLogWriter(setting);
            LogLevel = setting.LogLevel;
        }

        protected override ILogWriter Writer { get; }
        protected override LogLevel LogLevel { get; }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName, string memberName, LogLevel severity, string message, TState state)
        {
            if (_classesAndMethods.Count == 0)
            {
                return true;
            }
            if (_classesAndMethods.ContainsKey(categoryName))
            {
                var methodNames = _classesAndMethods[categoryName];
                if (methodNames.Length == 0 || methodNames.Contains(memberName))
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            if (_classesAndMethods.Count == 0)
            {
                return true;
            }
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
