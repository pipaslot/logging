using System;

namespace Pipaslot.Logging.Benchmark.Mocks
{
    public class NullLogWriter : ILogWriter
    {
        public void WriteLog(string log, DateTime dateTime, string traceIdentifier)
        {
            // Ignore operation
        }
    }
}