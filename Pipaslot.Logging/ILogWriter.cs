using Pipaslot.Logging.Records;

namespace Pipaslot.Logging
{
    /// <summary>
    ///     Provider writing LogScopes to filesystem or database
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Write log scope to target stream
        /// </summary>
        /// <param name="logRecords"></param>
        void WriteLog(LogScope logRecords);
    }
}