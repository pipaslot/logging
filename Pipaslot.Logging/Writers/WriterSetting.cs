using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Writers
{
    public class WriterSetting 
    {
        public string Path { get; }
        public string Filename { get; }
        public LogLevel LogLevel { get; }

        public WriterSetting(string path, string filename, LogLevel logLevel = LogLevel.Trace)
        {
            Path = path;
            Filename = filename;
            LogLevel = logLevel;
        }
    }
}