using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Groups
{
    public class LogGroup
    {
        public DateTimeOffset Time { get; } = DateTimeOffset.Now;

        public List<Log> Logs { get; } = new List<Log>();

        public int Depth => Logs.LastOrDefault()?.Depth ?? 0;

        public class Log
        {
            public string CategoryName { get; }
            public LogLevel Severity { get; }
            public string Message { get; }
            public object State { get; }
            public int Depth { get; }
            public DateTimeOffset Time { get; } = DateTimeOffset.Now;
            public Log(string categoryName, LogLevel severity, string message, object state, int depth)
            {
                CategoryName = categoryName;
                Severity = severity;
                Message = message;
                State = state;
                Depth = depth;
            }
        }
    }
}
