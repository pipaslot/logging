using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Groups
{
    public class LogGroup
    {
        private readonly List<Log> _logs = new List<Log>();
        public DateTimeOffset Time { get; } = DateTimeOffset.Now;
        public IReadOnlyCollection<Log> Logs => _logs;

        public int Depth => Logs.LastOrDefault()?.Depth ?? 0;

        public void Add(Log log)
        {
            _logs.Add(log);
        }

        public class Log
        {
            public Log(string categoryName, LogLevel severity, string message, object? state, int depth, bool shouldBeWritten)
            {
                CategoryName = categoryName;
                Severity = severity;
                Message = message;
                State = state;
                Depth = depth;
                ShouldBeWritten = shouldBeWritten;
            }

            public string CategoryName { get; }
            public LogLevel Severity { get; }
            public string Message { get; }
            public object? State { get; }
            public int Depth { get; }
            public DateTimeOffset Time { get; } = DateTimeOffset.Now;
            public bool ShouldBeWritten { get; }
        }
    }
}