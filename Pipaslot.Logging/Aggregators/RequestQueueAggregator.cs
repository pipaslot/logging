﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Aggregators
{
    /// <summary>
    ///     Logs all separated requests
    /// </summary>
    internal class RequestQueueAggregator : QueueAggregatorBase
    {
        public RequestQueueAggregator(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(writer, options)
        {
        }

        protected override Queue ProcessQueueBeforeWrite(Queue queue)
        {
            return queue.TraceIdentifier.StartsWith(Constants.CliTraceIdentifierPrefix)
                ? queue.CloneEmpty()
                : queue;
        }
    }
}