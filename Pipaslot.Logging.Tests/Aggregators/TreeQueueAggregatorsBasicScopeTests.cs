using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class TreeQueueAggregatorsBasicScopeTests : BasicScopeTests<TreeQueueAggregator>
    {
        protected override ILogger CreateLogger(ILogWriter writer)
        {
            return TestLoggerFactory.CreateLogger( IQueueAggregatorExtensions.Category,
                (o) => new TreeQueueAggregator(writer, o, IQueueAggregatorExtensions.Category));
        }
    }
}