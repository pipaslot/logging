using System.Collections.Generic;
using System.Threading.Tasks;
using Pipaslot.Logging.Records;

namespace Pipaslot.Logging
{
    /// <summary>
    ///     Service for sending LogScopes through network
    /// </summary>
    public interface ILogSender
    {
        Task SendLog(string message, IReadOnlyCollection<LogRecord> records);
    }
}