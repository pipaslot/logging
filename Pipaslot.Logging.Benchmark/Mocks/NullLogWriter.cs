using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Benchmark.Mocks
{
    public class NullLogWriter : ILogWriter
    {
        public void WriteLog(IQueue records)
        {
            // Ignore operation
        }
    }
}