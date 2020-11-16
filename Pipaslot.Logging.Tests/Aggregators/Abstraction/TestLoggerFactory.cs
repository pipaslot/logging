using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators.Abstraction
{
    internal static class TestLoggerFactory
    {
        public static PipaslotLogger CreateLogger<TAggregator>(string category, IHttpContextAccessor httpContextAccessor, Func<IOptions<PipaslotLoggerOptions>, TAggregator> factory) where TAggregator : IQueueAggregator
        {
            var options = new PipaslotLoggerOptionsMock();
            var queues = new IQueueAggregator[]
            {
                factory(options.Object)
            };
            return new PipaslotLogger(queues, httpContextAccessor, category);
        }

        public static PipaslotLogger CreateLogger<TAggregator>(string category, Func<IOptions<PipaslotLoggerOptions>, TAggregator> factory) where TAggregator : IQueueAggregator
        {
            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
            };
            var options = new PipaslotLoggerOptionsMock();
            var queues = new IQueueAggregator[]
            {
                factory(options.Object)
            };
            return new PipaslotLogger(queues, httpContextAccessor, category);
        }

        public static (ILogger first, ILogger second, ILogger third) CreateLogger<TAggregator>(string category1, string category2, string category3,
            Func<IOptions<PipaslotLoggerOptions>, TAggregator> factory) where TAggregator : IQueueAggregator
        {
            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
            };
            var options = new PipaslotLoggerOptionsMock();
            var queues = new IQueueAggregator[]
            {
                factory(options.Object)
            };
            return (new PipaslotLogger(queues, httpContextAccessor, category1),
                new PipaslotLogger(queues, httpContextAccessor, category2),
                new PipaslotLogger(queues, httpContextAccessor, category3));
        }

        private class HttpContextAccessor : IHttpContextAccessor
        {
            public HttpContext HttpContext { get; set; }
        }
    }
}
