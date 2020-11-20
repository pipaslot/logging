using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Filters;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Tests.Filters
{
    internal class RequestQueueFilterTests
    {
        [Test]
        public void AcceptOnlyRequestRecords()
        {
            var queue = new GrowingQueue("request")
            {
                RecordFactory.Create(0, RecordType.Record, LogLevel.Trace)
            };

            var filter = new RequestQueueFilter();
            var result = filter.Filter(queue);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void IgnoreCliQueue()
        {
            var queue = new GrowingQueue(Constants.CliTraceIdentifierPrefix)
            {
                RecordFactory.Create(0, RecordType.Record, LogLevel.Critical)
            };

            var filter = new RequestQueueFilter();
            var result = filter.Filter(queue);

            Assert.AreEqual(0, result.Count);
        }
    }
}