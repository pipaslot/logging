using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Pipaslot.Logging
{
    public class DefaultPipaslotLoggerOptions : IOptions<PipaslotLoggerOptions>
    {
        private readonly IConfiguration _config;

        private PipaslotLoggerOptions? _value;

        public DefaultPipaslotLoggerOptions(IConfiguration config)
        {
            _config = config;
        }

        public PipaslotLoggerOptions Value
        {
            get
            {
                if (_value == null){
                    _value = new PipaslotLoggerOptions();
                    _config.GetSection("Logging").GetSection("Pipaslot").Bind(_value);
                    if (string.IsNullOrWhiteSpace(_value.OutputPath)) throw new Exception("Missing configuration for key 'Logging.OutputPath', please check your appsettings file");
                }

                return _value;
            }
        }
    }
}