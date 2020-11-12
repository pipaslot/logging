using System;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Records
{
    /// <summary>
    /// Single input from logger
    /// </summary>
    public class LogRecord
    {
        public LogRecord(string categoryName, LogLevel severity, string message, object? state, int depth, LogType type)
        {
            CategoryName = categoryName;
            Severity = severity;
            Message = message;
            State = state;
            Depth = depth;
            Type = type;
        }

        public string CategoryName { get; }
        public LogLevel Severity { get; }
        public string Message { get; }
        public object? State { get; }
        public int Depth { get; }
        public LogType Type { get; }
        public DateTimeOffset Time { get; } = DateTimeOffset.Now;
    }
}