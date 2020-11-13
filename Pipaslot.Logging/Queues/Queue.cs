using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Set of log records collected together. Is read-only for consuming libraries
    /// </summary>
    public class Queue
    {
        private readonly List<Record> _logs = new List<Record>();

        internal Queue(string traceIdentifier)
        {
            TraceIdentifier = traceIdentifier;
        }
        private Queue(string traceIdentifier, DateTimeOffset time, IEnumerable<Record> logs)
        {
            TraceIdentifier = traceIdentifier;
            Time = time;
            _logs = logs.ToList();
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
        public IReadOnlyCollection<Record> Logs => _logs;

        internal int Depth => Logs.LastOrDefault()?.Depth ?? 0;

        internal void Add(Record record)
        {
            _logs.Add(record);
        }

        internal bool HasAnyWriteableLog()
        {
            return Logs.Any(l => l.Type == RecordType.Record);
        }

        internal Queue CloneWith(IEnumerable<Record> logs)
        {
            return new Queue(TraceIdentifier, Time, logs);
        }
    }
}