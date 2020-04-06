using System.IO;

namespace Pipaslot.Logging
{
    public interface ILogWriter
    {
        //TODO Consider using async
        void WriteLog(string log);
    }
}