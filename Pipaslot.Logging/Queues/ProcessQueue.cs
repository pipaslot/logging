using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.States;
using Pipaslot.Logging.Groups;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Log all processes together
    /// </summary>
    public class ProcessQueue : QueueBase
    {
        // private readonly LogGroupCollection _logGroups = new LogGroupCollection();
        // private readonly LogGroupFormatter _formatter = new LogGroupFormatter();
        protected override ILogWriter Writer { get; }

        public ProcessQueue(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
        }

        protected override bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName, LogLevel severity, TState state)
        {
            return traceIdentifier?.StartsWith(Constrants.CliTraceIdentifierPrefix) ?? false;
        }

        protected override bool CanWrite<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            return true;
        }
    }
}