using System.Threading.Tasks;

namespace Pipaslot.Logging
{
    public interface ILogSender
    {
        Task SendLogAsync(string log);
    }
}
