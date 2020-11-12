using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.States;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    [TestFixture]
    class SendQueueAggregatorsTests : BaseQueueAggregatorsTestsWithMinimalLogLevel<SendQueueAggregator>
    {
        protected override SendQueueAggregator CreateQueue(ILogWriter writer, LogLevel level)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new SendQueueAggregator(optionsMock.Object, level, writer);
        }

        protected override SendQueueAggregator CreateQueue(ILogWriter writer)
        {
            return CreateQueue(writer, LogLevel.Error);
        }
    }
}