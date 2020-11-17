using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Tests.Filters
{
    internal class TreeQueueFilterTests
    {
        private const string Categ1 = "FirstCategory";
        private const string Categ2 = "SecondCategory";
        private const string Ignore = "IgnoredCategory";

        [Test]
        public void WriteNestedNotSpecifiedCategory()
        {
            var records = new List<Record>();

            records.Add(RecordFactory.Create(Categ1, 1, "yes", RecordType.ScopeBeginIgnored));
            records.Add(RecordFactory.Create(Categ1, 1, "yes", RecordType.Record, LogLevel.Critical));

            records.Add(RecordFactory.Create(Ignore, 1, "yes", RecordType.Record, LogLevel.Critical));
            records.Add(RecordFactory.Create(Ignore, 2, "yes", RecordType.ScopeBeginIgnored));
            records.Add(RecordFactory.Create(Ignore, 2, "yes", RecordType.Record, LogLevel.Critical));
            records.Add(RecordFactory.Create(Ignore, 2, "yes", RecordType.ScopeEndIgnored));
            records.Add(RecordFactory.Create(Ignore, 1, "yes", RecordType.Record, LogLevel.Critical));

            records.Add(RecordFactory.Create(Categ2, 1, "yes", RecordType.Record, LogLevel.Critical));
            records.Add(RecordFactory.Create(Categ2, 2, "yes", RecordType.ScopeBeginIgnored));
            records.Add(RecordFactory.Create(Categ2, 2, "yes", RecordType.Record, LogLevel.Critical));
            records.Add(RecordFactory.Create(Categ2, 2, "yes", RecordType.ScopeEndIgnored));
            records.Add(RecordFactory.Create(Categ2, 1, "yes", RecordType.Record, LogLevel.Critical));

            records.Add(RecordFactory.Create(Ignore, 1, "yes", RecordType.Record, LogLevel.Critical));
            records.Add(RecordFactory.Create(Ignore, 2, "yes", RecordType.ScopeBeginIgnored));
            records.Add(RecordFactory.Create(Ignore, 2, "yes", RecordType.Record, LogLevel.Critical));
            records.Add(RecordFactory.Create(Ignore, 2, "yes", RecordType.ScopeEndIgnored));
            records.Add(RecordFactory.Create(Ignore, 1, "yes", RecordType.Record, LogLevel.Critical));

            records.Add(RecordFactory.Create(Categ1, 1, "yes", RecordType.Record, LogLevel.Critical));
            records.Add(RecordFactory.Create(Categ1, 1, "yes", RecordType.ScopeEndIgnored));

            RunAndAssert(records, 19, "yes");
        }

        [Test]
        public void IgnoreWrappingNotSpecifiedCategory()
        {
            var records = new List<Record>();
            
            records.Add(RecordFactory.Create(Ignore, 0, "no ", RecordType.Record, LogLevel.Critical));
            records.Add(RecordFactory.Create(Ignore, 1, "no ", RecordType.ScopeBeginIgnored));
            records.Add(RecordFactory.Create(Ignore, 1, "no ", RecordType.Record, LogLevel.Critical));

            records.Add(RecordFactory.Create(Categ1, 2, "yes", RecordType.ScopeBeginIgnored));
            records.Add(RecordFactory.Create(Categ1, 2, "yes", RecordType.Record, LogLevel.Critical));
            
            records.Add(RecordFactory.Create(Categ2, 2, "yes", RecordType.Record, LogLevel.Critical));
            records.Add(RecordFactory.Create(Categ2, 3, "yes", RecordType.ScopeBeginIgnored));
            records.Add(RecordFactory.Create(Categ2, 3, "yes", RecordType.Record, LogLevel.Critical));
            records.Add(RecordFactory.Create(Categ2, 3, "yes", RecordType.ScopeEndIgnored));
            records.Add(RecordFactory.Create(Categ2, 2, "yes", RecordType.Record, LogLevel.Critical));
            
            records.Add(RecordFactory.Create(Categ1, 2, "yes", RecordType.Record, LogLevel.Critical));
            records.Add(RecordFactory.Create(Categ1, 2, "yes", RecordType.ScopeEndIgnored));
            
            records.Add(RecordFactory.Create(Ignore, 1, "no ", RecordType.Record, LogLevel.Critical));
            records.Add(RecordFactory.Create(Ignore, 1, "no ", RecordType.ScopeEndIgnored));
            records.Add(RecordFactory.Create(Ignore, 0, "no ", RecordType.Record, LogLevel.Critical));

            RunAndAssert(records, 9, "yes");
        }

        private void RunAndAssert(List<Record> records, int expectedCount, string expectedMessage)
        {
            var queue = new Queue("", DateTimeOffset.Now, records);
            var filter = new TreeQueueFilter(Categ1, Categ2);
            var result = filter.Filter(queue);

            Assert.AreEqual(expectedCount, result.Count);
            Assert.IsTrue(result.All(l => l.Message == expectedMessage));
        }
    }
}