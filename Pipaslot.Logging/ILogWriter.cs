using System;
using System.IO;

namespace Pipaslot.Logging
{
    /// <summary>
    /// Provider wring LogGroups to filesystem or database
    /// </summary>
    public interface ILogWriter
    {
        void WriteLog(string log, DateTime dateTime, string traceIdentifier);
    }
}