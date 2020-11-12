using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    [TestFixture]
    class FlatQueueAggregatorsTests : BaseQueueAggregatorsTestsWithMinimalLogLevel<FlatQueueAggregator>
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