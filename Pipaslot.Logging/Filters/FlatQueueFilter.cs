﻿using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Filters
{
    /// <summary>
    ///     Logging only for defined classes/scopes and their methods. Does not involve also deeper logging.
    /// </summary>
    internal class FlatQueueFilter : IQueueFilter
    {
        private readonly LogLevel _logLevel;

        public FlatQueueFilter(LogLevel logLevel)
        {
            _logLevel = logLevel;
        }

        public Queue Filter(Queue queue)
        {
            var records = new List<Record>(queue.Count);
            int? endDepth = null;
            foreach (var log in queue)
            {
                if (log.Type == RecordType.Record)
                {
                    if (_logLevel <= log.Severity)
                    {
                        records.Add(log);
                        if(endDepth == null){
                            endDepth = log.Depth;
                        }
                    }
                }
                else if (endDepth != null && log.Depth == endDepth.Value && log.Type == RecordType.ScopeEndIgnored)
                {
                    //ignore last scope from the same depth
                }
                else if (endDepth != null && log.Depth > endDepth.Value)
                {
                    // Involve nested scopes
                    records.Add(log);
                }
            }
            return queue.CloneWith(records);
        }

    }
}