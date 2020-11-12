using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Queues
{
    [TestFixture]
    class FlatQueueTests : BaseQueueTestsWithMinimalLogLevel<FlatQueueAggregator>
    {
        [Test]
        public void TODO()
        {
            throw new NotImplementedException();
        }

        protected override FlatQueueAggregator CreateQueue(ILogWriter writer, LogLevel level)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new FlatQueueAggregator(writer, level, optionsMock.Object);
        }

        protected override FlatQueueAggregator CreateQueue(ILogWriter writer)
        {
            return CreateQueue(writer, LogLevel.Error);
        }
    }
}