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
        public DeepWriter(WriterSetting setting) : base(setting)
        {
        }

        public DeepWriter(string path, string filename, Dictionary<string, string[]> classesAndMethods) : base(new WriterSetting(path, filename))
        {
            Setting.ClassesAndMethods = classesAndMethods;
        }

        public DeepWriter(string path, string filename, string className, string[] methodNames = null, LogLevel logLevel = LogLevel.Information) : base(new WriterSetting(path, filename))
        {
            Setting.ClassesAndMethods.Add(className, methodNames ?? new string[0]);
            Setting.LogLevel = logLevel;
        }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName, string memberName, LogLevel severity, string message, TState state)
        {
            return true;
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            var ics = state as IncreaseScopeState;
            if(Setting.ClassesAndMethods.ContainsKey(categoryName) && ics != null)
            {
                var methodNames = Setting.ClassesAndMethods[categoryName];
                if (methodNames.Length == 0 || methodNames.Contains(ics.CallerMemberName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
