using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.States;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Writers
{
    /// <summary>
    /// Log all processes together
    /// </summary>
    public class ProcessWriter : IWriter
    {
        public WriterSetting Setting { get; set; }
        protected readonly QueueCollection _queues = new QueueCollection();
        private readonly object _fileLock = new object();
        private readonly QueueFormatter _formatter = new QueueFormatter();
        public ProcessWriter(WriterSetting setting)
        {
            Setting = setting;
            if (!Directory.Exists(setting.Path))
            {
                Directory.CreateDirectory(setting.Path);
            }
        }

        public ProcessWriter(string path, string filename) : this(new WriterSetting(path, filename))
        {
        }

        public void Write<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {
            if (!IsCLI(traceIdentifier))
            {
                return;
            }
            var queue = _queues.GetQueue(traceIdentifier, true);
            if (queue == null)
            {
                // Log should be ommited
                return;
            }
            // Update depth
            var depth = queue.Depth;
            var stateType = typeof(TState);
            var logStepToFile = true;
            if (stateType == typeof(IncreaseScopeState))
            {
                depth++;
            }
            else if (stateType == typeof(DecreaseScopeState))
            {
                depth--;
                logStepToFile = false;
            }

            var logRow = new Queue.Log(categoryName, severity, message, state, depth);
            if (logStepToFile)
            {
                var previousDepth = queue.Logs.LastOrDefault()?.Depth ?? 0;
                var log = _formatter.FormatRecord(previousDepth, depth, logRow);
                WriteToFile(traceIdentifier, log);
                if (previousDepth <= 0)
                {
                    queue.Logs.Clear();
                }
            }
            queue.Logs.Add(logRow);
        }
        private bool IsCLI(string traceIdentifier)
        {
            return traceIdentifier?.StartsWith("CLI:") ?? false;
        }

        private StreamWriter GetStream(string id)
        {
            var formattedId = id.Replace(":", "-");
            var fileName = Regex.Replace(Setting.Filename, "{date}", DateTime.Now.ToString("yyyyMMdd"), RegexOptions.IgnoreCase);
            fileName = Regex.Replace(fileName, "{id}", formattedId, RegexOptions.IgnoreCase);
            var path = Path.Combine(Setting.Path, fileName);

            if (File.Exists(path))
            {
                return File.AppendText(path);
            }
            return File.CreateText(path);
        }

        protected void WriteToFile(string id, string log)
        {
            if (!string.IsNullOrWhiteSpace(log))
            {
                lock (_fileLock)
                {
                    using (var stream = GetStream(id))
                    {
                        stream.WriteLine(log);
                    }
                }
            }
        }

        public void Dispose()
        {
            _queues.Dispose();
        }
    }
}
