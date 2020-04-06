using System.Threading.Tasks;

namespace Pipaslot.Logging
{
    public interface ILogSender
    {
        //Todo COnsider using sync
        Task SendLogAsync(string log);
    }
}
