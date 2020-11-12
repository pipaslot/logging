using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Queues
{
    [TestFixture]
    class FlatQueueTests : BaseQueueTestsWithMinimalLogLevel<FlatQueue>
    {
        [Test]
        public void TODO()
        {
            throw new NotImplementedException();
        }

        protected override FlatQueue CreateQueue(ILogWriter writer, LogLevel level)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new FlatQueue(writer, level, optionsMock.Object);
        }

        protected override FlatQueue CreateQueue(ILogWriter writer)
        {
            return CreateQueue(writer, LogLevel.Error);
        }
    }
}