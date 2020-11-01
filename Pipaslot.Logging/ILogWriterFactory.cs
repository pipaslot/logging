namespace Pipaslot.Logging
{
    public interface ILogWriterFactory
    {
        ILogWriter Create(string fileName);
    }
}