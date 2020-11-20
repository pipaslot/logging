using Microsoft.Extensions.Options;
using Pipaslot.Logging.Configuration;

namespace Pipaslot.Logging.Writers
{
    /// <summary>
    ///     Default file writer factory implementation
    /// </summary>
    public class FileWriterFactory : IFileWriterFactory
    {
        private readonly IFileNameFormatter _fileNameFormatter;
        private readonly IOptions<PipaslotLoggerOptions> _options;

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