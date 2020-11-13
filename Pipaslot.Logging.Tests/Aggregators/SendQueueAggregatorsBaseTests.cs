using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class SendQueueAggregatorsBaseTests : BaseTests<SendQueueAggregator>
    {
        protected override SendQueueAggregator CreateQueue(ILogWriter writer)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new SendQueueAggregator(optionsMock.Object, LogLevel.Error, writer);
        }
    }
}