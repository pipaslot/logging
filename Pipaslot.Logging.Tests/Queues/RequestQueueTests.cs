using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Tests.Queues
{
    [TestFixture]
    class RequestQueueTests : BaseQueueTests<RequestQueue>
    {
        [Test]
        public void TODO()
        {
            throw new NotImplementedException();
        }

        protected override RequestQueue CreateQueue(ILogWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}