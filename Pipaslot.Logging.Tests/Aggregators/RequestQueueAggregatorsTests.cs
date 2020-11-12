using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;

namespace Pipaslot.Logging.Tests.Aggregators
{
    [TestFixture]
    class RequestQueueAggregatorsTests : BaseQueueAggregatorsTests<RequestQueueAggregator>
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