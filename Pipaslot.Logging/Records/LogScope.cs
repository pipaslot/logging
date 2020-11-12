using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipaslot.Logging.Records
{
    public class LogScope
    {
        private readonly List<LogRecord> _logs = new List<LogRecord>();
        public DateTimeOffset Time { get; } = DateTimeOffset.Now;
        public IReadOnlyCollection<LogRecord> Logs => _logs;

        public int Depth => Logs.LastOrDefault()?.Depth ?? 0;

        public void Add(LogRecord logRecord)
        {
            _logs.Add(logRecord);
        }
    }
}