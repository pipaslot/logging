using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class ProcessQueueAggregatorsIncludingWrappingScopeInResultTests : IncludingWrappingScopeInResultTests
    {
        protected override string TraceId => Constants.CliTraceIdentifierPrefix + "trace";

        protected override PipaslotLogger CreateLogger(ILogWriter writer)
        {
            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = null
            };
            return TestLoggerFactory.CreateLogger(IQueueAggregatorExtensions.Category, httpContextAccessor,
                (o) => new ProcessQueueAggregator(writer, o));
        }
    }
}