using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipaslot.Logging.Queues
{

    /// <summary>
    /// Set of log records collected together
    /// </summary>
    public class GrowingQueue : IQueue
    {
        private readonly List<Record> _logs = new List<Record>();

        internal GrowingQueue(string traceIdentifier)
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
        /// Current log level depth
        /// </summary>
        internal int Depth { get; private set; }

        internal void Add(Record record)
        {
            _logs.Add(record);
            if (record.Type != RecordType.ScopeEndIgnored)
            {
                Depth = record.Depth;
            }
        }
        /// <summary>
        /// Returns true if at least one message (not a scope) is written
        /// </summary>
        /// <returns></returns>
        public bool HasAnyRecord()
        {
            return _logs.Any(l => l.Type == RecordType.Record);
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
