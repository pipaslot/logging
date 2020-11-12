using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;

namespace Pipaslot.Logging.Tests.Aggregators
{
    [TestFixture]
    class ProcessQueueAggregatorsTests  : BaseQueueAggregatorsTests<ProcessQueueAggregator>
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