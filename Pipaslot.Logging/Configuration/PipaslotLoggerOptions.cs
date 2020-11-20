// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Pipaslot.Logging.Configuration
{
    /// <summary>
    /// Basic configuration options for logger
    /// </summary>
    public class PipaslotLoggerOptions
    {
        public string OutputPath { get; set; } = "./logs";
        public bool IncludeScopes { get; set; }
        public bool IncludeMethods { get; set; }
    }
}