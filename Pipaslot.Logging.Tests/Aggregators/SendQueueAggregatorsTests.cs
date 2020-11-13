using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class SendQueueAggregatorsTests
    {
        [Test]
        public void WriteOnlyInfoInNestedScopes_ShouldIgnore()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseScope();
                queue.WriteLog(LogLevel.Information);
                queue.WriteIncreaseScope();
                queue.WriteLog(LogLevel.Information);
                queue.WriteDecreaseScope();
                queue.WriteLog(LogLevel.Information);
                queue.WriteDecreaseScope();
                writerMock.VerifyWriteLogIsNotCalled();
            }
        }

        [Test]
        public void WriteErrorInNestedScope_ShouldWrite()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseScope();
                queue.WriteLog(LogLevel.Information);
                queue.WriteIncreaseScope();
                queue.WriteLog(LogLevel.Error);
                queue.WriteDecreaseScope();
                queue.WriteLog(LogLevel.Information);
                queue.WriteDecreaseScope();
                writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(6);
            }
        }

        private SendQueueAggregator CreateQueue(ILogWriter writer, LogLevel level = LogLevel.Error)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new SendQueueAggregator(optionsMock.Object, level, writer);
        }
    }
}