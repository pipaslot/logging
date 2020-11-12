using Microsoft.Extensions.Options;

namespace Pipaslot.Logging
{
    /// <summary>
    /// Default file writer factory implementation
    /// </summary>
    public class FileWriterFactory : IFileWriterFactory
    {
        private readonly IOptions<PipaslotLoggerOptions> _options;

        public FileWriterFactory(IOptions<PipaslotLoggerOptions> options)
        {
            _options = options;
        }

        /// <inheritdoc />
        public ILogWriter Create(string fileName)
        {
            return new FileWriter(_options, fileName);
        }
    }
}