using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class ProcessQueueAggregatorsBasicScopeTests : BasicScopeTests<ProcessQueueAggregator>
    {
        protected override ILogger CreateLogger(ILogWriter writer)
        {
            return TestLoggerFactory.CreateLogger( IQueueAggregatorExtensions.Category,
                (o) => new ProcessQueueAggregator(writer,  o));
        }
    }
}