using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pipaslot.Logging.Queues
{
    public class EmptyQueue : IQueue
    {
        public EmptyQueue(string traceIdentifier, DateTimeOffset time)
        {
            TraceIdentifier = traceIdentifier;
            Time = time;
        }

        public IEnumerator<Record> GetEnumerator()
        {
            return Enumerable.Empty<Record>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => 0;
        public string TraceIdentifier { get; }
        public DateTimeOffset Time { get; }

        public bool HasAnyRecord()
        {
            return false;
        }
    }
}