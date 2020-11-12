using System;
using System.Collections.Generic;
using Pipaslot.Logging.Records;

namespace Pipaslot.Logging.Benchmark.Mocks
{
    public class NullLogWriter : ILogWriter
    {
        public void WriteLog(string log, DateTime dateTime, string traceIdentifier, IReadOnlyCollection<LogRecord> logRecords)
        {
            // Ignore operation
        }
    }
}