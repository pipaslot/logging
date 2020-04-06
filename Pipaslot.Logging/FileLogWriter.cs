using System;
using System.IO;
using System.Text.RegularExpressions;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging
{
    public class FileLogWriter : ILogWriter
    {
        private readonly object _fileLock = new object();
        private readonly WriterSetting _setting;

        public FileLogWriter(WriterSetting setting)
        {
            _setting = setting;
            if (!Directory.Exists(setting.Path))
            {
                Directory.CreateDirectory(setting.Path);
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
            var fileName = Regex.Replace(_setting.Filename, "{date}", dateTime.ToString("yyyyMMdd"), RegexOptions.IgnoreCase);
            fileName = Regex.Replace(fileName, "{id}", id, RegexOptions.IgnoreCase);
            var path = Path.Combine(_setting.Path, fileName);

            if (File.Exists(path))
            {
                return File.AppendText(path);
            }

            return File.CreateText(path);
        }
    }
}