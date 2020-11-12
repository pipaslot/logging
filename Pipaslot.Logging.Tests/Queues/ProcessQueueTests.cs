using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Tests.Queues
{
    [TestFixture]
    class ProcessQueueTests  : BaseQueueTests<ProcessQueue>
    {
        [Test]
        public void TODO()
        {
            throw new NotImplementedException();
        }

        protected override ProcessQueue CreateQueue(ILogWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}