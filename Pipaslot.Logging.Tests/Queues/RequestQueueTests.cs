using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;

namespace Pipaslot.Logging.Tests.Queues
{
    [TestFixture]
    class RequestQueueTests : BaseQueueTests<RequestQueueAggregator>
    {
        [Test]
        public void TODO()
        {
            throw new NotImplementedException();
        }

        protected override RequestQueueAggregator CreateQueue(ILogWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}