using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipaslot.Logging.Records
{
    /// <summary>
    /// Set of log records collected together. Is read-only for consuming libraries
    /// </summary>
    public class LogScope
    {
        private readonly List<LogRecord> _logs = new List<LogRecord>();

        internal LogScope(string traceIdentifier)
        {
            TraceIdentifier = traceIdentifier;
        }
        /// <summary>
        /// Request or process trace identifier
        /// </summary>
        public string TraceIdentifier { get; }

        /// <summary>
        /// First record creation date
        /// </summary>
        public DateTimeOffset Time { get; } = DateTimeOffset.Now;

        /// <summary>
        /// Collected log messages
        /// </summary>
        public IReadOnlyCollection<LogRecord> Logs => _logs;

        internal int Depth => Logs.LastOrDefault()?.Depth ?? 0;

        internal void Add(LogRecord logRecord)
        {
            _logs.Add(logRecord);
        }
    }
}