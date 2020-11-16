using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class SendQueueAggregatorsIncludingWrappingScopeInResultTests : IncludingWrappingScopeInResultTests
    {
        protected override string TraceId => IQueueAggregatorExtensions.TraceId;

        protected override PipaslotLogger CreateLogger(ILogWriter writer)
        {
            return TestLoggerFactory.CreateLogger( IQueueAggregatorExtensions.Category,
                (o) => new SendQueueAggregator(o, LogLevel.Error, writer));
        }
    }
}