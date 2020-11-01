namespace Pipaslot.Logging.Benchmark.Mocks
{
    public class NullLogWriterFactory : ILogWriterFactory
    {
        public ILogWriter Create(string fileName)
        {
            return new NullLogWriter();
        }
    }
}