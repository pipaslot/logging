using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class TreeQueueAggregatorsBaseTests : BaseTests<TreeQueueAggregator>
    {
        protected override TreeQueueAggregator CreateQueue(ILogWriter writer)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new TreeQueueAggregator(writer, optionsMock.Object, IQueueAggregatorExtensions.Category);
        }
    }
}