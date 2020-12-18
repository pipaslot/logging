using System;

namespace Pipaslot.Logging.Configuration
{
    public class LogFileEraseHostedServiceOptions
    {
        public TimeSpan MaxAge { get; set; } = TimeSpan.FromDays(90);
        public TimeSpan RepeatInterval { get; set; } = TimeSpan.FromDays(1);
    }
}
