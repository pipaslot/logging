using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Records;

namespace Pipaslot.Logging
{
    /// <summary>
    /// Default file writer implementation containing locking for unique access
    /// </summary>
    public class FileWriter : ILogWriter
    {
        private readonly object _fileLock = new object();
        private readonly string _filename;
        private readonly IOptions<PipaslotLoggerOptions> _options;
        private readonly LogScopeFormatter _formatter;

        public FileWriter(IOptions<PipaslotLoggerOptions> options, string filename) : this(options, filename, new LogScopeFormatter())
        {
        }

        public FileWriter(IOptions<PipaslotLoggerOptions> options, string filename, LogScopeFormatter formatter)
        {
            _options = options;
            _filename = filename;
            _formatter = formatter;
        }

        /// <summary>
        /// Write log to file if nto empty
        /// </summary>
        public void WriteLog(LogScope scope)
        {
            var log = _formatter.Format(scope);
            if (!string.IsNullOrWhiteSpace(log))
            {
                lock (_fileLock)
                {
                    using var stream = GetStream(scope.Time.DateTime);
                    stream.WriteLine(log);
                }
            }
        }

        private StreamWriter GetStream(DateTime dateTime)
        {
            var outputPath = _options.Value.OutputPath;
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
            var fileName = Regex.Replace(_filename, "{date}", dateTime.ToString("yyyyMMdd"), RegexOptions.IgnoreCase);
            var path = Path.Combine(outputPath, fileName);

            return File.Exists(path) ? File.AppendText(path) : File.CreateText(path);
        }
    }
}