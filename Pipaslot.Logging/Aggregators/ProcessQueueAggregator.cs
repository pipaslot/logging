using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Aggregators
{
    /// <summary>
    ///     LogRecord all processes together
    /// </summary>
    internal class ProcessQueueAggregator : QueueAggregatorBase
    {
        public ProcessQueueAggregator(ILogWriter writer, IOptions<PipaslotLoggerOptions> options) : base(options)
        {
            Writer = writer;
        }

        //public void WriteLog<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        //{
        //    if (CanBeWrittenIntoQueue(traceIdentifier))
        //    {
        //        var queue = Queues.GetQueue(traceIdentifier, false);
        //        if (queue == null)
        //        {
        //            // Write directly if is nto part of some queue bordered by scope
        //            var group = new Queue(traceIdentifier);
        //            group.Add(new Record(categoryName, severity, message, state, group.Depth, RecordType.Record));
        //            _writer.WriteLog(group);
        //            return;
        //        }

        //        queue.Add(new Record(categoryName, severity, message, state, queue.Depth, RecordType.Record));
        //    }
        //}

        //public void WriteScopeChange<TState>(string traceIdentifier, string categoryName, TState state)
        //{
        //    if (CanBeWrittenIntoQueue(traceIdentifier))
        //    {
        //        var queue = Queues.GetQueue(traceIdentifier, true);
        //        if (queue == null){
        //            // LogRecord should be omitted
        //            return;
        //        }

        //        // Update depth
        //        var depth = queue.Depth;
        //        var logType = QueueAggregatorBase.GetLogType<TState>(_options.Value);
        //        if (logType == RecordType.ScopeBegin || logType == RecordType.ScopeBeginIgnored){
        //            depth++;
        //        }
        //        else if (logType == RecordType.ScopeEndIgnored){
        //            depth--;
        //        }

        //        // LogRecord or finish
        //        if (depth <= 0){
        //            // Remove request history from memory 
        //            Queues.Remove(traceIdentifier);
        //            if(queue.HasAnyWriteableLog()){
        //                _writer.WriteLog(queue);
        //            }
        //        }
        //        else{
        //            //Write only increasing scopes and ignore decreasing scopes
        //            queue.Add(new Record(categoryName, LogLevel.None, "", state, depth, logType));
        //        }
        //    }
        //}
        

        protected override bool CanWriteRootLogWithoutScope()
        {
            return true;
        }

        protected override ILogWriter Writer { get; }


        protected override bool CanCreateNewLogScope(string traceIdentifier, string categoryName, LogLevel severity)
        {
            return traceIdentifier?.StartsWith(Constants.CliTraceIdentifierPrefix) ?? false;
        }

        protected override bool CanAddIntoExistingLogScope(string traceIdentifier, string categoryName, LogLevel severity, Queue queue)
        {
            return traceIdentifier?.StartsWith(Constants.CliTraceIdentifierPrefix) ?? false;
        }
    }
}