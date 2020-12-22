using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging
{
    class RequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PipaslotLoggerProvider _provider;
        private readonly ILogger<RequestLoggerMiddleware> _logger;

        public RequestLoggerMiddleware(RequestDelegate next, PipaslotLoggerProvider provider, ILogger<RequestLoggerMiddleware> logger)
        {
            _next = next;
            _provider = provider;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var aggregator = _provider.GetAggregator();
            var traceIdentifier = context.TraceIdentifier;
            aggregator.BeginQueue(traceIdentifier);
            var timer = new Stopwatch();
            timer.Start();
            _logger.LogInformation($"Request starting {context.Request.Protocol} {context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
            try
            {
                await _next(context);
            }
            finally
            {
                timer.Stop();
                _logger.LogInformation($"Request finished in {timer.ElapsedMilliseconds}ms {context.Response.StatusCode} {context.Response.ContentType}");
                aggregator.EndQueue(traceIdentifier);
            }
        }
    }

}
