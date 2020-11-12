using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;

namespace Pipaslot.Logging.Tests.Queues
{
    [TestFixture]
    class ProcessQueueTests  : BaseQueueTests<ProcessQueueAggregator>
    {
        [Test]
        public void TODO()
        {
            throw new NotImplementedException();
        }

        protected override ProcessQueueAggregator CreateQueue(ILogWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}