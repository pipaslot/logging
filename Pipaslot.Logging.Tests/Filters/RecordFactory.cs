using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Tests.Filters
{
    static class RecordFactory
    {
        public static Record Create(int depth, RecordType type, LogLevel level = LogLevel.None)
        {
            return new Record("category", level, "message", null, depth, type);
        }

        public static Record Create(int depth, string message, RecordType type, LogLevel level = LogLevel.None)
        {
            return new Record("category", level, message, null, depth, type);
        }
        
        public static Record Create(string category, int depth, string message, RecordType type, LogLevel level = LogLevel.None)
        {
            return new Record(category, level, message, null, depth, type);
        }

    }
}
