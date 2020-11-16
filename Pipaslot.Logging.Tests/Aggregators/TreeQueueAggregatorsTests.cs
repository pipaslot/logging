using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class TreeQueueAggregatorsTests
    {
        private const string FirstCategory = "FirstCategory";
        private const string SecondCategory = "SecondCategory";
        private const string IgnoredCategory = "IgnoredCategory";

        [TestCase(true, 19)]
        [TestCase(false, 9)]
        public void WriteNestedNotSpecifiedCategory(bool addNested, int expectedCount)
        {
            var writerMock = new LogWritterMock();
            var (firstCategoryLogger, secondCategoryLogger, ignoredCategoryLogger) = CreateLogger(writerMock.Object, FirstCategory, SecondCategory, IgnoredCategory);

            using (firstCategoryLogger.BeginMethod())
            {
                firstCategoryLogger.LogCritical("message");
                if (addNested)
                {
                    ignoredCategoryLogger.LogCritical("message");
                    using (ignoredCategoryLogger.BeginScope(null))
                    {
                        ignoredCategoryLogger.LogCritical("message");
                    }
                    ignoredCategoryLogger.LogCritical("message");
                }

                secondCategoryLogger.LogCritical("message");
                using (secondCategoryLogger.BeginScope(null))
                {
                    secondCategoryLogger.LogCritical("message");
                }
                secondCategoryLogger.LogCritical("message");

                if (addNested)
                {
                    ignoredCategoryLogger.LogCritical("message");
                    using (ignoredCategoryLogger.BeginScope(null))
                    {
                        ignoredCategoryLogger.LogCritical("message");
                    }
                    ignoredCategoryLogger.LogCritical("message");
                }
                firstCategoryLogger.LogCritical("message");
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(expectedCount);
        }

        [TestCase(true, 9)]
        [TestCase(false, 9)]
        public void IgnoreWrappingNotSpecifiedCategory(bool addWrapping, int expectedCount)
        {

            var writerMock = new LogWritterMock();
            var (firstCategoryLogger, secondCategoryLogger, ignoredCategoryLogger) = CreateLogger(writerMock.Object, FirstCategory, SecondCategory, IgnoredCategory);

            IDisposable wrappingScope = null;

            if (addWrapping)
            {
                wrappingScope = ignoredCategoryLogger.BeginScope(null);
                ignoredCategoryLogger.LogCritical("message");
            }

            using (firstCategoryLogger.BeginMethod())//causes write
            {
                firstCategoryLogger.LogCritical("message");//causes write

                secondCategoryLogger.LogCritical("message");//causes write
                using (secondCategoryLogger.BeginScope(null))//causes write
                {
                    secondCategoryLogger.LogCritical("message");//causes write
                }//causes write
                secondCategoryLogger.LogCritical("message");//causes write

                firstCategoryLogger.LogCritical("message");//causes write
            }

            if (addWrapping)
            {
                ignoredCategoryLogger.LogCritical("message");
                wrappingScope?.Dispose();
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(expectedCount);
        }
        private (ILogger first, ILogger second, ILogger third) CreateLogger(ILogWriter writer, string category1, string category2, string ignoredCategory)
        {
            return TestLoggerFactory.CreateLogger(category1, category2, ignoredCategory,
                (o) => new TreeQueueAggregator(writer, o, category1, category2));
        }
    }
}