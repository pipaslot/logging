using System;
using System.Collections.Generic;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Aggregated records from logger
    /// </summary>
    public interface IQueue : IReadOnlyCollection<Record>
    {
        /// <summary>
        /// Request or process trace identifier
        /// </summary>
        string TraceIdentifier { get; }

        /// <summary>
        /// First record creation date
        /// </summary>
        DateTimeOffset Time { get; }
        
        /// <summary>
        /// Return true if contains any Record with RecordType.Record
        /// </summary>
        /// <returns></returns>
        bool HasAnyRecord();
    }
}