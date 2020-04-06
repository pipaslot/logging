using System.IO;

namespace Pipaslot.Logging.Writers
{
    public interface ILogMessageWriter
    {
        void WriteToFile(string log);
    }
}