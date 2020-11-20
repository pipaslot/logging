using Pipaslot.Logging.Queues;

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
        /// <param name="records"></param>
        void WriteLog(IQueue records);
    }
}