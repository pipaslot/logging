using System.Threading.Tasks;

namespace Pipaslot.Logging.Writers
{
    public interface ILogSender
    {
        Task SendLogAsync(string log);
    }
}
