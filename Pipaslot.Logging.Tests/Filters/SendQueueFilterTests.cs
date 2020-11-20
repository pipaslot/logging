using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Filters;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Filters
{
    internal class SendQueueFilterTests
    {
        [Test]
        public void WriteSingle_HigherLevel_OneMessageIsInserted()
        {
            var queue = new GrowingQueue("")
            {
                RecordFactory.Create(3,RecordType.Record,LogLevel.Error)
            };

            var filter = new SendQueueFilter(LogLevel.Error);
            var result = filter.Filter(queue);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void WriteScopeAndLog_LogHasLowPriority_IgnoreScope()
        {
            var queue = new GrowingQueue("")
            {
                RecordFactory.Create(1,RecordType.ScopeBeginIgnored),
                RecordFactory.Create(1,RecordType.Record,LogLevel.Information)
            };

            var filter = new SendQueueFilter(LogLevel.Error);
            var result = filter.Filter(queue);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void WriteOnlyInfoInNestedScopes_ShouldIgnore()
        {
            var queue = new GrowingQueue("")
            {
                RecordFactory.Create(1,RecordType.ScopeBeginIgnored),
                RecordFactory.Create(1,RecordType.Record,LogLevel.Information),
                RecordFactory.Create(2,RecordType.ScopeBeginIgnored),
                RecordFactory.Create(2,RecordType.Record,LogLevel.Information),
                RecordFactory.Create(2,RecordType.ScopeEndIgnored),
                RecordFactory.Create(1,RecordType.Record,LogLevel.Information),
                RecordFactory.Create(1,RecordType.ScopeEndIgnored),
            };

            var filter = new SendQueueFilter(LogLevel.Error);
            var result = filter.Filter(queue);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void WriteErrorInNestedScope_ShouldWrite()
        {
            var queue = new GrowingQueue("")
            {
                RecordFactory.Create(1,RecordType.ScopeBeginIgnored),
                RecordFactory.Create(1,RecordType.Record,LogLevel.Information),
                RecordFactory.Create(2,RecordType.ScopeBeginIgnored),
                RecordFactory.Create(2,RecordType.Record,LogLevel.Error),
                RecordFactory.Create(2,RecordType.ScopeEndIgnored),
                RecordFactory.Create(1,RecordType.Record,LogLevel.Information),
                RecordFactory.Create(1,RecordType.ScopeEndIgnored),
            };

            var filter = new SendQueueFilter(LogLevel.Error);
            var result = filter.Filter(queue);

            Assert.AreEqual(7, result.Count);
        }
    }
}