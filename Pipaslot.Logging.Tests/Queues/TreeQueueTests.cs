using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Queues
{
    [TestFixture]
    class TreeQueueTests : BaseQueueTests<TreeQueue>
    {
        [Test]
        public void TODO()
        {
            throw new NotImplementedException();
        }

        protected override TreeQueue CreateQueue(ILogWriter writer)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new TreeQueue(writer, optionsMock.Object);
        }
    }
}