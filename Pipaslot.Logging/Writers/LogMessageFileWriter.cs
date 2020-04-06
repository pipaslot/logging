using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Pipaslot.Logging.Writers
{
    public class LogMessageFileWriter : ILogMessageWriter
    {
        private readonly object _fileLock = new object();
        private readonly WriterSetting _setting;

        public LogMessageFileWriter(WriterSetting setting)
        {
            _setting = setting;
            if (!Directory.Exists(setting.Path))
            {
                Directory.CreateDirectory(setting.Path);
            }
        }

        public void WriteToFile(string log)
        {
            if (!string.IsNullOrWhiteSpace(log))
            {
                lock (_fileLock)
                {
                    using (var stream = GetStream())
                    {
                        stream.WriteLine(log);
                    }
                }
            }
        }

        private StreamWriter GetStream()
        {
            var fileName = Regex.Replace(_setting.Filename, "{date}", DateTime.Now.ToString("yyyyMMdd"),
                RegexOptions.IgnoreCase);
            var path = Path.Combine(_setting.Path, fileName);

            if (File.Exists(path))
            {
                return File.AppendText(path);
            }

            return File.CreateText(path);
        }
    }
}