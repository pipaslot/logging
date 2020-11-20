using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Filters;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Tests.Filters
{
    internal class FlatQueueFilterTests
    {
        [TestCase(LogLevel.Critical, 1)]
        [TestCase(LogLevel.Error, 2)]
        [TestCase(LogLevel.Warning, 3)]
        [TestCase(LogLevel.Information, 4)]
        [TestCase(LogLevel.Debug, 5)]
        [TestCase(LogLevel.Trace, 6)]
        public void RecordsOnly_FilterWithLowerPriorities(LogLevel minimalLevel, int expected)
        {
            var queue = new GrowingQueue("")
            {
                RecordFactory.Create(0, RecordType.Record, LogLevel.Critical),
                RecordFactory.Create(0, RecordType.Record, LogLevel.Error),
                RecordFactory.Create(0, RecordType.Record, LogLevel.Warning),
                RecordFactory.Create(0, RecordType.Record, LogLevel.Information),
                RecordFactory.Create(0, RecordType.Record, LogLevel.Debug),
                RecordFactory.Create(0, RecordType.Record, LogLevel.Trace)
            };

            var filter = new FlatQueueFilter(minimalLevel);
            var result = filter.Filter(queue);

            Assert.AreEqual(expected, result.Count);
        }

        [TestCase(LogLevel.Critical, 1)]
        [TestCase(LogLevel.Error, 2)]
        [TestCase(LogLevel.Warning, 3)]
        [TestCase(LogLevel.Information, 4)]
        [TestCase(LogLevel.Debug, 5)]
        [TestCase(LogLevel.Trace, 6)]
        public void ScopedRecords_FilterOnlyRecordsWithLowerPriorities(LogLevel minimalLevel, int expected)
        {
            var queue = new GrowingQueue("")
            {
                RecordFactory.Create(1, RecordType.ScopeBeginIgnored),
                RecordFactory.Create(1, RecordType.Record, LogLevel.Critical),
                RecordFactory.Create(1, RecordType.Record, LogLevel.Error),
                RecordFactory.Create(1, RecordType.Record, LogLevel.Warning),
                RecordFactory.Create(1, RecordType.Record, LogLevel.Information),
                RecordFactory.Create(1, RecordType.Record, LogLevel.Debug),
                RecordFactory.Create(1, RecordType.Record, LogLevel.Trace),
                RecordFactory.Create(1, RecordType.ScopeEndIgnored, 0)
            };

            var filter = new FlatQueueFilter(minimalLevel);
            var result = filter.Filter(queue);

            Assert.AreEqual(expected, result.Count);
        }

        [Test]
        public void NestedScopesWithRecords_KeepOnlyWithMinimalLogLevelStaringOnFirstMessageOccurrence()
        {
            var queue = new GrowingQueue("")
            {
                RecordFactory.Create(0, "no ", RecordType.Record, LogLevel.Information),

                RecordFactory.Create(1, "no ", RecordType.ScopeBeginIgnored),
                RecordFactory.Create(1, "no ", RecordType.Record, LogLevel.Information),

                RecordFactory.Create(2, "no ", RecordType.ScopeBeginIgnored),
                RecordFactory.Create(2, "yes", RecordType.Record, LogLevel.Error),
                RecordFactory.Create(2, "no ", RecordType.Record, LogLevel.Information),

                RecordFactory.Create(3, "no ", RecordType.ScopeBeginIgnored),
                RecordFactory.Create(3, "no ", RecordType.Record, LogLevel.Information),
                RecordFactory.Create(3, "yes", RecordType.Record, LogLevel.Error),
                RecordFactory.Create(3, "no ", RecordType.ScopeEndIgnored),

                RecordFactory.Create(2, "yes", RecordType.Record, LogLevel.Error),
                RecordFactory.Create(2, "no ", RecordType.ScopeEndIgnored),

                RecordFactory.Create(1, "no ", RecordType.Record, LogLevel.Information),
                RecordFactory.Create(1, "no ", RecordType.ScopeEndIgnored),

                RecordFactory.Create(0, "no ", RecordType.Record, LogLevel.Information)
            };

            var filter = new FlatQueueFilter(LogLevel.Error);
            var result = filter.Filter(queue);

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(l => l.Message == "yes"));
        }
    }
}