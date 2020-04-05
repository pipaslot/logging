using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Writers
{
    public class WriterSetting
    {
        public string Path { get; }
        public string Filename { get; }
        
        /// <summary>
        /// Definition of classes and their methods to be tracked
        /// </summary>
        public Dictionary<string, string[]> ClassesAndMethods = new Dictionary<string, string[]>();

        public LogLevel LogLevel { get; set; } = LogLevel.Trace;

        public WriterSetting(string path, string filename)
        {
            Path = path;
            Filename = filename;
        }
    }
}
