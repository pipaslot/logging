using System.IO;

namespace Pipaslot.Logging
{
    public interface ILogWriter
    {
        void WriteLog(string log);
    }
}