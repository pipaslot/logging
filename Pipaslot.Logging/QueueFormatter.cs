using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging
{
    /// <summary>
    ///     Default log file formatter
    /// </summary>
    public class QueueFormatter
    {
        /// <summary>
        ///     Format whole logRecord group belonging to the same scope
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public virtual string Format(IQueue queue)
        {
            var sb = new StringBuilder();
            sb.Append(queue.Time);
            sb.Append(" ");
            sb.AppendLine(queue.TraceIdentifier);

            var previousDepth = 0;
            var rows = 0;
            foreach (var log in queue){
                if (log.Type == RecordType.Record || log.Type == RecordType.ScopeBegin){
                    sb.AppendLine(FormatRecord(log, previousDepth, log.Depth));
                    rows++;
                }

                previousDepth = log.Depth;
            }

            if (rows > 0) return sb.ToString();

            return "";
        }

        /// <summary>
        ///     Format severity
        /// </summary>
        /// <param name="severity">Message severity</param>
        public virtual string FormatSeverity(LogLevel severity)
        {
            var codes = new Dictionary<LogLevel, string>
            {
                {LogLevel.Trace, "[TRC]"},
                {LogLevel.Debug, "[DEB]"},
                {LogLevel.Information, "[INF]"},
                {LogLevel.Warning, "[WRN]"},
                {LogLevel.Error, "[ERR]"},
                {LogLevel.Critical, "[FTL]"},
                {LogLevel.None, "     "}
            };
            if (codes.ContainsKey(severity)) return codes[severity];
            return "[INF]";
        }

        /// <summary>
        ///     Format single log record message
        /// </summary>
        /// <param name="record">LogRecord record</param>
        /// <param name="previousDepth"></param>
        /// <param name="currentDepth">Current scope depth</param>
        public virtual string FormatRecord(Record record, int previousDepth, int currentDepth)
        {
            var sb = new StringBuilder();
            sb.Append(record.Time.ToString("HH:mm:ss.fff"));
            sb.Append(" ");
            sb.Append(FormatSeverity(record.Severity));
            sb.Append(" ");
            sb.Append(FormatDepth(previousDepth, currentDepth, record.Type));
            if (record.State != null && record.State is IState state) sb.Append(state.FormatMessage(record.CategoryName, record.Message));
            sb.Append(record.Message);

            if (record.State != null && !(record.State is IState)){
                var serializedData = SerializeState(record.State);
                sb.Append(" ");
                sb.Append(serializedData);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Format depth shown between severity and message
        /// </summary>
        /// <param name="previousDepth"></param>
        /// <param name="currentDepth"></param>
        /// <returns></returns>
        protected virtual string FormatDepth(int previousDepth, int currentDepth, RecordType recordType)
        {
            var sb = new StringBuilder();
            for (var i = 3; i < currentDepth; i++){
                sb.Append("| ");
            }

            if (currentDepth > 1){
                if (previousDepth < currentDepth && recordType != RecordType.Record)
                    sb.Append("+ ");
                else
                    sb.Append("| ");
            }

            return sb.ToString();
        }

        protected virtual string SerializeState(object data)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.SerializeObject(data, settings);
        }
    }
}