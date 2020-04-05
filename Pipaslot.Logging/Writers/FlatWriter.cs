using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Writers
{
    /// <summary>
    /// Logging only for defined classes/scopes and their methods. Does not involve also deeper logging.
    /// </summary>
    public class FlatWriter : WriterBase
    {
        public FlatWriter(string path, string filename, string className, string[] methodNames, LogLevel logLevel = LogLevel.Information) : base(new WriterSetting(path, filename))
        {
            Setting.ClassesAndMethods.Add(className, methodNames.ToArray());
            Setting.LogLevel = logLevel;
        }

        public FlatWriter(string path, string filename, LogLevel logLevel = LogLevel.Information) : base(new WriterSetting(path, filename))
        {
            Setting.LogLevel = logLevel;
        }


        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName, string memberName, LogLevel severity, string message, TState state)
        {
            if (Setting.ClassesAndMethods.Count == 0)
            {
                return true;
            }
            if (Setting.ClassesAndMethods.ContainsKey(categoryName))
            {
                var methodNames = Setting.ClassesAndMethods[categoryName];
                if (methodNames.Length == 0 || methodNames.Contains(memberName))
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            if (Setting.ClassesAndMethods.Count == 0)
            {
                return true;
            }
            var ics = state as IncreaseScopeState;
            if (Setting.ClassesAndMethods.ContainsKey(categoryName) && ics != null)
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
