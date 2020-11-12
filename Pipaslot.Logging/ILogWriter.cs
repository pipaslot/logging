using System;
using System.Collections.Generic;
using Pipaslot.Logging.Records;

namespace Pipaslot.Logging
{
    /// <summary>
    ///     Provider wring LogScopes to filesystem or database
    /// </summary>
    public interface ILogWriter
    {
        void WriteLog(string log, DateTime dateTime, string traceIdentifier, IReadOnlyCollection<LogRecord> logRecords);
    }
}