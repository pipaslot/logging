using System.Threading.Tasks;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging
{
    /// <summary>
    ///     Service for sending LogScopes through network
    /// </summary>
    public interface ILogSender
    {
        Task SendLog(Queue queue);
    }
}