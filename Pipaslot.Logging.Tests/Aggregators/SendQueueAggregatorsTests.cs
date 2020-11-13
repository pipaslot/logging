using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    [TestFixture]
    class SendQueueAggregatorsTests : BaseQueueAggregatorsTestsWithMinimalLogLevel<SendQueueAggregator>
    {
        [Test]
        public void WriteOnlyInfoInNestedScopes_ShouldIgnore()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseScope();
                queue.WriteLog( LogLevel.Information );
                queue.WriteIncreaseScope();
                queue.WriteLog( LogLevel.Information);
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
                queue.WriteLog( LogLevel.Information);
                queue.WriteIncreaseScope();
                queue.WriteLog(LogLevel.Error);
                queue.WriteDecreaseScope();
                queue.WriteLog( LogLevel.Information);
                queue.WriteDecreaseScope();
                writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(6);
            }
        }
        protected override SendQueueAggregator CreateQueue(ILogWriter writer, LogLevel level)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new SendQueueAggregator(optionsMock.Object, level, writer);
        }

        protected override SendQueueAggregator CreateQueue(ILogWriter writer)
        {
            return CreateQueue(writer, LogLevel.Error);
        }
    }
}