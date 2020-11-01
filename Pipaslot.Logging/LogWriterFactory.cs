using Microsoft.Extensions.Options;

namespace Pipaslot.Logging
{
    public class LogWriterFactory : ILogWriterFactory
    {
        private IOptions<PipaslotLoggerOptions> _options;

        public LogWriterFactory(IOptions<PipaslotLoggerOptions> options)
        {
            _options = options;
        }

        public ILogWriter Create(string fileName)
        {
            return new FileLogWriter(_options, fileName);
        }
    }
}