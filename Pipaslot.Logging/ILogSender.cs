using System.Threading.Tasks;

namespace Pipaslot.Logging
{
    public interface ILogSender
    {
        void SendLog(string log);
    }
}
