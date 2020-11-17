using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Set of log records collected together. Is read-only for consuming libraries
    /// </summary>
    public class Queue : IReadOnlyCollection<Record>
    {
        private readonly List<Record> _logs = new List<Record>();

        internal Queue(string traceIdentifier)
        {
            TraceIdentifier = traceIdentifier;
        }
        internal Queue(string traceIdentifier, DateTimeOffset time, IEnumerable<Record> logs)
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
        [Obsolete("Use direct enumerator on this object")]
        public IReadOnlyCollection<Record> Logs => _logs;

        /// <summary>
        /// Current log level depth
        /// </summary>
        internal int Depth
        {
            get
            {
                var last = _logs.LastOrDefault();
                if (last != null)
                {
                    if (last.Type == RecordType.ScopeEndIgnored)
                    {
                        return last.Depth - 1;
                    }
                    return last.Depth;
                }
                return 0;
            }
        }

        internal void Add(Record record)
        {
            _logs.Add(record);
        }
        /// <summary>
        /// Returns true if at least one message (not a scope) is written
        /// </summary>
        /// <returns></returns>
        internal bool HasAnyRecord()
        {
            return _logs.Any(l => l.Type == RecordType.Record);
        }

        internal Queue CloneWith(IEnumerable<Record> logs)
        {
            return new Queue(TraceIdentifier, Time, logs);
        }
        internal Queue CloneEmpty()
        {
            return new Queue(TraceIdentifier, Time, new Record[0]);
        }

        public IEnumerator<Record> GetEnumerator()
        {
            return _logs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// Amount of records written into queue
        /// </summary>
        public int Count => _logs.Count;
    }
}