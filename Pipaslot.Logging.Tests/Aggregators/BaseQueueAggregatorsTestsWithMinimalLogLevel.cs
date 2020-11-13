using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.States;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    abstract class BaseQueueAggregatorsTestsWithMinimalLogLevel<TQueue> : BaseQueueAggregatorsTests<TQueue> where TQueue : IQueueAggregator
    {
        [Test]
        public void WriteSingle_HigherLevel_OneMessageIsInserted()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object, LogLevel.Information)){
                queue.WriteLog( LogLevel.Error);
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(1);
        }
        
        [Test]
        public void WriteScopeAndLog_LogHasLowPriority_IgnoreScope()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object, LogLevel.Error)){
                queue.WriteIncreaseScope();
                queue.WriteLog(LogLevel.Information);
            }

            writerMock.VerifyWriteLogIsNotCalled();
        }
        
        [Test]
        public void WriteMethodAndLog_LogHasLowPriority_IgnoreScope()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object, LogLevel.Error)){
                queue.WriteIncreaseMethod();
                queue.WriteLog(LogLevel.Information);
            }

            writerMock.VerifyWriteLogIsNotCalled();
        }
        
        protected abstract TQueue CreateQueue(ILogWriter writer, LogLevel level);
    }
}