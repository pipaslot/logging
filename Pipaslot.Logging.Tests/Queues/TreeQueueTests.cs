using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Queues
{
    [TestFixture]
    class TreeQueueTests : BaseQueueTests<TreeQueueAggregator>
    {
        [Test]
        public void TODO()
        {
            throw new NotImplementedException();
        }

        protected override TreeQueueAggregator CreateQueue(ILogWriter writer)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new TreeQueueAggregator(writer, optionsMock.Object);
        }
    }
}