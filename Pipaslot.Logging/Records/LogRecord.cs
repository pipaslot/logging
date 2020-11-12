using System;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Records
{
    /// <summary>
    /// Single input from logger
    /// </summary>
    public class LogRecord
    {
        public LogRecord(string categoryName, LogLevel severity, string message, object? state, int depth, bool shouldBeWritten)
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