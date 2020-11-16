using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class FlatQueueAggregatorsBasicScopeTests : BasicScopeTests<FlatQueueAggregator>
    {
        protected override ILogger CreateLogger(ILogWriter writer)
        {
            return TestLoggerFactory.CreateLogger(IQueueAggregatorExtensions.Category,
                (o) => new FlatQueueAggregator(writer, LogLevel.Error, o));
        }
    }
}