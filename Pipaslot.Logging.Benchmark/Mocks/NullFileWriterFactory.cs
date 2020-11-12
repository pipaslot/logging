namespace Pipaslot.Logging.Benchmark.Mocks
{
    public class NullFileWriterFactory : IFileWriterFactory
    {
        public ILogWriter Create(string fileName)
        {
            return new NullLogWriter();
        }
    }
}