using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Queues
{
    public class QueueFormatter
    {
        public string FormatRequest(Queue queue, string requestIdentifier, LogLevel logSinceLevel)
        {
            var sb = new StringBuilder();
            sb.Append(queue.Time);
            sb.Append(" ");
            sb.AppendLine(requestIdentifier);

            var previousDepth = 0;
            var first = true;
            var rows = 0;
            foreach (var log in queue.Logs)
            {
                if (log.Depth < previousDepth)
                {
                    // Decreasing depth - ignore this case
                }
                else if ((int)log.Severity < (int)logSinceLevel)
                {
                    // Log message has too low severity
                }
                else if (first && log.CategoryName == "Microsoft.AspNetCore.Hosting.Internal.WebHost")
                {
                    // Ignore first log which is Webhost call without usefull informations
                }
                else
                {
                    sb.AppendLine(FormatRecord(previousDepth, log.Depth, log));
                    rows++;
                }
                previousDepth = log.Depth;
                first = false;
            }
            if (rows > 0)
            {
                return sb.ToString();
            }

            return "";
        }
        
        protected string FormatSeverity(LogLevel severity)
        {
            var codes = new Dictionary<LogLevel, string>
            {
                {LogLevel.Trace, "[TRC]" },
                {LogLevel.Debug, "[DEB]" },
                {LogLevel.Information, "[INF]" },
                {LogLevel.Warning, "[WRN]" },
                {LogLevel.Error, "[ERR]" },
                {LogLevel.Critical, "[FTL]" },
                {LogLevel.None, "" }
            };
            if (codes.ContainsKey(severity))
            {
                return codes[severity];
            }
            return "[INF]";
        }


        public string FormatRecord(int previousDepth, int currentDepth, Queue.Log log)
        {
            var sb = new StringBuilder();
            sb.Append(log.Time.ToString("HH:mm:ss.fff"));
            sb.Append(" ");
            sb.Append(FormatSeverity(log.Severity));
            sb.Append(" ");
            sb.Append(FormatDepth(previousDepth, currentDepth));
            if (log.State != null && log.State is IState state)
            {
                sb.Append(state.FormatMessage(log.CategoryName, log.Message));
            }
            sb.Append(log.Message);


            if (log.State != null && !(log.State is IState))
            {
                var serializedData = Serialize(log.State);
                sb.Append(" ");
                sb.Append(serializedData);
            }

            return sb.ToString();
        }

        protected string FormatDepth(int previousDepth, int currentDepth)
        {
            var sb = new StringBuilder();
            for (var i = 2; i < currentDepth; i++)
            {
                sb.Append("| ");
            }

            if (previousDepth < currentDepth)
            {
                if (currentDepth > 1){
                    sb.Append("+ ");
                }
            }
            else if (previousDepth > currentDepth)
            {
                sb.Append("/ ");
            }
            else if (currentDepth > 1)
            {
                sb.Append("| ");
            }

            return sb.ToString();
        }

        protected string Serialize(object data)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.SerializeObject(data, settings);
        }
    }
}
