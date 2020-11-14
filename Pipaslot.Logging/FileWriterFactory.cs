using Microsoft.Extensions.Options;

namespace Pipaslot.Logging
{
    /// <summary>
    /// Default file writer factory implementation
    /// </summary>
    public class FileWriterFactory : IFileWriterFactory
    {
        private readonly IOptions<PipaslotLoggerOptions> _options;
        private readonly IFileNameFormatter _fileNameFormatter;

        public FileWriterFactory(IOptions<PipaslotLoggerOptions> options, IFileNameFormatter fileNameFormatter)
        {
            _options = options;
            _fileNameFormatter = fileNameFormatter;
        }

        /// <inheritdoc />
        public ILogWriter Create(string name, RollingInterval rollingInterval)
        {
            return new FileWriter(_options, name, rollingInterval, _fileNameFormatter);
        }
    }
}