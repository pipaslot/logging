using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class RequestQueueAggregatorsBaseTests : BaseTests<RequestQueueAggregator>
    {
        protected override RequestQueueAggregator CreateQueue(ILogWriter writer)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new RequestQueueAggregator(writer, optionsMock.Object);
        }
    }
}