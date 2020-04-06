using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Writers
{
    /// <summary>
    /// Logging for defined classes/scopes and their methods. Involves also all deeper logging
    /// </summary>
    public class DeepWriter : WriterBase
    {
        /// <summary>
        /// Definition of classes and their methods to be tracked
        /// </summary>
        private readonly Dictionary<string, string[]> _classesAndMethods = new Dictionary<string, string[]>();
        
        public DeepWriter(string path, string filename, Dictionary<string, string[]> classesAndMethods) : this(new WriterSetting(path, filename))
        {
            _classesAndMethods = classesAndMethods ?? new Dictionary<string, string[]>();;
        }

        public DeepWriter(string path, string filename, string className, string[] methodNames = null, LogLevel logLevel = LogLevel.Information) : this(new WriterSetting(path, filename, logLevel))
        {
            _classesAndMethods.Add(className, methodNames ?? new string[0]);
        }
        
        public DeepWriter(WriterSetting setting)
        {
            MessageWriter = new LogMessageFileWriter(setting);
            LogLevel = setting.LogLevel;
        }

        protected override ILogMessageWriter MessageWriter { get; }
        protected override LogLevel LogLevel { get; }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName, string memberName, LogLevel severity, string message, TState state)
        {
            return true;
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            var ics = state as IncreaseScopeState;
            if(_classesAndMethods.ContainsKey(categoryName) && ics != null)
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
