﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    ///     Log all processes together
    /// </summary>
    public class ProcessQueue : QueueBase
    {
        public ProcessQueue(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
        }

        protected override ILogWriter Writer { get; }

        protected override bool CanCreateNewQueue(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return traceIdentifier?.StartsWith(Constants.CliTraceIdentifierPrefix) ?? false;
        }

        protected override bool CanWriteIntoExistingQueue(string categoryName, LogLevel severity)
        {
            return true;
        }
    }
}