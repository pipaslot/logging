using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging
{
    public class FileLogWriter : ILogWriter
    {
        private readonly IOptions<PipaslotLoggerOptions> _options;
        private readonly string _filename;
        private readonly object _fileLock = new object();

        public FileLogWriter(IOptions<PipaslotLoggerOptions> options, string filename)
        {
            _options = options;
            _filename = filename;
        }

        public void WriteLog(string log, DateTime dateTime, string traceIdentifier)
        {
            if (!string.IsNullOrWhiteSpace(log))
            {
                lock (_fileLock)
                {
                    using (var stream = GetStream(dateTime, traceIdentifier))
                    {
                        stream.WriteLine(log);
                    }
                }
            }
        }

        private StreamWriter GetStream(DateTime dateTime, string traceIdentifier)
        {
            var outputPath = _options.Value.OutputPath;
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            var id = traceIdentifier?.Replace(":", "-") ?? "";
            var fileName = Regex.Replace(_filename, "{date}", dateTime.ToString("yyyyMMdd"), RegexOptions.IgnoreCase);
            fileName = Regex.Replace(fileName, "{id}", id, RegexOptions.IgnoreCase);
            var path = Path.Combine(outputPath, fileName);

            if (File.Exists(path))
            {
                return File.AppendText(path);
            }

            return File.CreateText(path);
        }
    }
}