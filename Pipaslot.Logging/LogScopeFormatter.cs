﻿using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pipaslot.Logging.Records;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging
{
    public class LogScopeFormatter
    {
        /// <summary>
        /// Format whole logRecord group belonging to the same scope
        /// </summary>
        /// <param name="logScope"></param>
        /// <returns></returns>
        public virtual string Format(LogScope logScope)
        {
            var sb = new StringBuilder();
            sb.Append(logScope.Time);
            sb.Append(" ");
            sb.AppendLine(logScope.TraceIdentifier);

            var previousDepth = 0;
            var rows = 0;
            foreach (var log in logScope.Logs){
                if (log.Type == LogType.Record || log.Type == LogType.ScopeBegin){
                    sb.AppendLine(FormatRecord(log, previousDepth, log.Depth));
                    rows++;
                }

                previousDepth = log.Depth;
            }

            if (rows > 0) return sb.ToString();

            return "";
        }

        /// <summary>
        /// Format severity
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
        /// Format single log record message
        /// </summary>
        /// <param name="logRecord">LogRecord record</param>
        /// <param name="previousDepth"></param>
        /// <param name="currentDepth">Current scope depth</param>
        public virtual string FormatRecord(LogRecord logRecord, int previousDepth, int currentDepth)
        {
            var sb = new StringBuilder();
            sb.Append(logRecord.Time.ToString("HH:mm:ss.fff"));
            sb.Append(" ");
            sb.Append(FormatSeverity(logRecord.Severity));
            sb.Append(" ");
            sb.Append(FormatDepth(previousDepth, currentDepth));
            if (logRecord.State != null && logRecord.State is IState state) sb.Append(state.FormatMessage(logRecord.CategoryName, logRecord.Message));
            sb.Append(logRecord.Message);
            
            if (logRecord.State != null && !(logRecord.State is IState)){
                var serializedData = SerializeState(logRecord.State);
                sb.Append(" ");
                sb.Append(serializedData);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Format depth shown between severity and message
        /// </summary>
        /// <param name="previousDepth"></param>
        /// <param name="currentDepth"></param>
        /// <returns></returns>
        protected virtual string FormatDepth(int previousDepth, int currentDepth)
        {
            var sb = new StringBuilder();
            for (var i = 2; i < currentDepth; i++){
                sb.Append("| ");
            }

            if (previousDepth < currentDepth){
                if (currentDepth > 1) sb.Append("+ ");
            }
            else if (previousDepth > currentDepth)
                sb.Append("/ ");
            else if (currentDepth > 1) sb.Append("| ");

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