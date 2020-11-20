using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pipaslot.Logging.Queues
{
    public class FixedSizeQueue : IQueue
    {
        private readonly IEnumerable<Record> _records;

        public FixedSizeQueue(string traceIdentifier, params Record[] records) : this(traceIdentifier, DateTimeOffset.Now, records)
        {
        }

        public FixedSizeQueue(string traceIdentifier, DateTimeOffset time, IEnumerable<Record> records)
        {
            _records = records;
            Time = time;
            TraceIdentifier = traceIdentifier;
        }

        public IEnumerator<Record> GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _records.Count();
        public string TraceIdentifier { get; }
        public DateTimeOffset Time { get; }
        public bool HasAnyRecord()
        {
            return this.Any(l => l.Type == RecordType.Record);
        }
    }
}