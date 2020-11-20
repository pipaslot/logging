using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Filters;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Filters
{
    internal class ProcessQueueFilterTests
    {
        [Test]
        public void AcceptsOnlyCliRecords()
        {
            var queue = new GrowingQueue(Constants.CliTraceIdentifierPrefix)
            {
                RecordFactory.Create( 0,RecordType.Record,LogLevel.Trace)
            };

            var filter = new ProcessQueueFilter();
            var result = filter.Filter(queue);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void IgnoreRequestQueue()
        {
            var queue = new GrowingQueue("request")
            {
                RecordFactory.Create( 0,RecordType.Record,LogLevel.Critical)
            };

            var filter = new ProcessQueueFilter();
            var result = filter.Filter(queue);

            Assert.AreEqual(0, result.Count);
        }
    }
}