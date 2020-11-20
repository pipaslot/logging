using Pipaslot.Logging.Queues;
using Pipaslot.Logging.Writers;

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