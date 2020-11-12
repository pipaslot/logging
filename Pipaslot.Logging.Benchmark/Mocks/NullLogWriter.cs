using Pipaslot.Logging.Records;

namespace Pipaslot.Logging.Benchmark.Mocks
{
    public class NullLogWriter : ILogWriter
    {
        public void WriteLog(LogScope logRecords)
        {
            // Ignore operation
        }
    }
}