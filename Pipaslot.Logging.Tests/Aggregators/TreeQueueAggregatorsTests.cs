using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class TreeQueueAggregatorsTests
    {
        private const string FirstCategory = "SecondCategory";
        private const string SecondCategory = "SecondCategory";
        private const string NestedCategory = "IgnoredCategory";

        [TestCase(true, 18)]
        [TestCase(false, 8)]
        public void WriteNestedNotSPecifiedCategory(bool addNested, int expectedCount)
        {
            var trace = "trace";
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseMethod(trace, FirstCategory);
                queue.WriteLog(LogLevel.Critical, trace, FirstCategory);

                if (addNested){
                    queue.WriteLog(LogLevel.Critical, trace, NestedCategory);
                    queue.WriteIncreaseScope(trace, NestedCategory);
                    queue.WriteLog(LogLevel.Critical, trace, NestedCategory);
                    queue.WriteDecreaseScope(trace, NestedCategory);
                    queue.WriteLog(LogLevel.Critical, trace, NestedCategory);
                }

                queue.WriteLog(LogLevel.Critical, trace, SecondCategory);
                queue.WriteIncreaseScope(trace, SecondCategory);
                queue.WriteLog(LogLevel.Critical, trace, SecondCategory);
                queue.WriteDecreaseScope(trace, SecondCategory);
                queue.WriteLog(LogLevel.Critical, trace, SecondCategory);

                if (addNested){
                    queue.WriteLog(LogLevel.Critical, trace, NestedCategory);
                    queue.WriteIncreaseScope(trace, NestedCategory);
                    queue.WriteLog(LogLevel.Critical, trace, NestedCategory);
                    queue.WriteDecreaseScope(trace, NestedCategory);
                    queue.WriteLog(LogLevel.Critical, trace, NestedCategory);
                }

                queue.WriteLog(LogLevel.Critical, trace, FirstCategory);
                queue.WriteDecreaseScope(trace, FirstCategory);
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(expectedCount);
        }

        [TestCase(true, 6)]
        [TestCase(false, 6)]
        public void IgnoreWrappingNotSpecifiedCategory(bool addWrapping, int expectedCount)
        {
            var trace = "trace";
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                if (addWrapping){
                    queue.WriteIncreaseScope(trace, NestedCategory);
                    queue.WriteLog(LogLevel.Critical, trace, NestedCategory);
                }

                queue.WriteIncreaseMethod(trace, FirstCategory);
                queue.WriteLog(LogLevel.Critical, trace, FirstCategory);

                queue.WriteIncreaseScope(trace, SecondCategory);
                queue.WriteLog(LogLevel.Critical, trace, SecondCategory);
                queue.WriteDecreaseScope(trace, SecondCategory);

                queue.WriteLog(LogLevel.Critical, trace, FirstCategory);
                queue.WriteDecreaseScope(trace, FirstCategory);

                if (addWrapping){
                    queue.WriteLog(LogLevel.Critical, trace, NestedCategory);
                    queue.WriteDecreaseScope(trace, NestedCategory);
                }
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(expectedCount);
        }

        private TreeQueueAggregator CreateQueue(ILogWriter writer)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new TreeQueueAggregator(writer, optionsMock.Object, FirstCategory, SecondCategory);
        }
    }
}