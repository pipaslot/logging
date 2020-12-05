using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    ///     Set of log records collected together
    /// </summary>
    public class GrowingQueue : IQueue
    {
        private readonly List<Record> _logs = new List<Record>();

        internal GrowingQueue(string traceIdentifier)
        {
            TraceIdentifier = traceIdentifier;
        }

        /// <summary>
        ///     Current log level depth
        /// </summary>
        internal int Depth { get; private set; }

        /// <summary>
        ///     First state used for increase scope
        /// </summary>
        internal IState? IncreaseScopeState { get; private set; }

        /// <summary>
        ///     Request or process trace identifier
        /// </summary>
        public string TraceIdentifier { get; }

        /// <summary>
        ///     First record creation date
        /// </summary>
        public DateTimeOffset Time { get; } = DateTimeOffset.Now;

        /// <summary>
        ///     Returns true if at least one message (not a scope) is written
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
        ///     Amount of records written into queue
        /// </summary>
        public int Count => _logs.Count;


        internal void Add(Record record, int? queueDepth = null, IState? increaseScopeState = null)
        {
            _logs.Add(record);
            if (queueDepth.HasValue)
            {
                Depth = queueDepth.Value;
            }
            if (increaseScopeState != null && IncreaseScopeState == null)
            {
                IncreaseScopeState = increaseScopeState;
            }
        }
    }
}