using System;
using System.IO;
using System.Text.RegularExpressions;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging
{
    public class FileLogWriter : ILogWriter
    {
        private readonly string _path;
        private readonly string _filename;
        private readonly object _fileLock = new object();

        public FileLogWriter(string path, string filename)
        {
            _path = path;
            _filename = filename;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
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
            var id = traceIdentifier?.Replace(":", "-") ?? "";
            var fileName = Regex.Replace(_filename, "{date}", dateTime.ToString("yyyyMMdd"), RegexOptions.IgnoreCase);
            fileName = Regex.Replace(fileName, "{id}", id, RegexOptions.IgnoreCase);
            var path = Path.Combine(_path, fileName);

            if (File.Exists(path))
            {
                return File.AppendText(path);
            }

            return File.CreateText(path);
        }
    }
}