using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Benchmark.Mocks
{
    public class NullLogWriter : ILogWriter
    {
        public void WriteLog(Queue records)
        {
            // Ignore operation
        }
    }
}