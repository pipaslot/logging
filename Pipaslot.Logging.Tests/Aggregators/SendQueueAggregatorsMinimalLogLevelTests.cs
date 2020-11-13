using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class SendQueueAggregatorsMinimalLogLevelTests : MinimalLogLevelTests<SendQueueAggregator>
    {
        protected override SendQueueAggregator CreateQueue(ILogWriter writer, LogLevel level)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new SendQueueAggregator(optionsMock.Object, level, writer);
        }
    }
}