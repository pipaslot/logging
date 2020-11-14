using System;
using System.IO;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging
{
    /// <summary>
    /// Default file writer implementation containing locking for unique access
    /// </summary>
    public class FileWriter : ILogWriter
    {
        private readonly object _fileLock = new object();
        private readonly string _name;
        private readonly IOptions<PipaslotLoggerOptions> _options;
        private readonly QueueFormatter _formatter;
        private readonly RollingInterval _rollingInterval;
        private readonly IFileNameFormatter _fileNameFormatter;

        public FileWriter(IOptions<PipaslotLoggerOptions> options, string name, RollingInterval rollingInterval, IFileNameFormatter fileNameFormatter) : this(options, name,
            rollingInterval, fileNameFormatter, new QueueFormatter())
        {
        }

        public FileWriter(IOptions<PipaslotLoggerOptions> options, string name, RollingInterval rollingInterval, IFileNameFormatter fileNameFormatter, QueueFormatter formatter)
        {
            _options = options;
            _name = name;
            _formatter = formatter;
            _rollingInterval = rollingInterval;
            _fileNameFormatter = fileNameFormatter;
        }

        /// <summary>
        /// Write log to file if nto empty
        /// </summary>
        public void WriteLog(Queue queue)
        {
            var log = _formatter.Format(queue);
            if (!string.IsNullOrWhiteSpace(log)){
                lock (_fileLock){
                    using var stream = GetStream(queue.Time.DateTime);
                    stream.WriteLine(log);
                }
            }
        }

        private StreamWriter GetStream(DateTime dateTime)
        {
            var outputPath = _options.Value.OutputPath;
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
            var fileName = _fileNameFormatter.Format(dateTime, _name, _rollingInterval);
            var path = Path.Combine(outputPath, fileName);

            return File.Exists(path) ? File.AppendText(path) : File.CreateText(path);
        }
    }
}