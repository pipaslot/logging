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

        // public override void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message,
        //     TState state)
        // {
        //     if (!IsCLI(traceIdentifier))
        //     {
        //         return;
        //     }
        //
        //     var queue = _logGroups.GetQueue(traceIdentifier, true);
        //     if (queue == null)
        //     {
        //         // Log should be ommited
        //         return;
        //     }
        //
        //     // Update depth
        //     var depth = queue.Depth;
        //     var stateType = typeof(TState);
        //     var logStepToFile = true;
        //     if (stateType == typeof(IncreaseScopeState))
        //     {
        //         depth++;
        //     }
        //     else if (stateType == typeof(DecreaseScopeState))
        //     {
        //         depth--;
        //         logStepToFile = false;
        //     }
        //
        //     var logRow = new LogGroup.Log(categoryName, severity, message, state, depth);
        //     if (logStepToFile)
        //     {
        //         var previousDepth = queue.Logs.LastOrDefault()?.Depth ?? 0;
        //         var log = _formatter.FormatRecord(previousDepth, depth, logRow);
        //         Writer.WriteLog(log, logRow.Time.DateTime, traceIdentifier);
        //         if (previousDepth <= 0)
        //         {
        //             queue.Logs.Clear();
        //         }
        //     }
        //
        //     queue.Logs.Add(logRow);
        // }

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