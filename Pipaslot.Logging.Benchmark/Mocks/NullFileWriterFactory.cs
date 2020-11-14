namespace Pipaslot.Logging.Benchmark.Mocks
{
    public class NullFileWriterFactory : IFileWriterFactory
    {
        public ILogWriter Create(string name, RollingInterval rollingInterval)
        {
            return new NullLogWriter();
        }
    }
}