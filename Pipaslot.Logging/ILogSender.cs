using System.Threading.Tasks;

namespace Pipaslot.Logging
{
    /// <summary>
    /// Service for sending LogGroups through network
    /// </summary>
    public interface ILogSender
    {
        void SendLog(string log);
    }
}
