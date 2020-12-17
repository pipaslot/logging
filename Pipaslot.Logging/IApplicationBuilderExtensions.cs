using Microsoft.AspNetCore.Builder;
using Pipaslot.Logging.Aggregators;

namespace Pipaslot.Logging
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Register middleware carrying about writing logs aggregated per request
        /// </summary>
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<RequestLoggerMiddleware>();
            QueueAggregator.CanCreateQueueFromScopes = false;
            return builder;
        }
    }
}
