using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.States;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    abstract class BaseQueueAggregatorsTests<TQueue> where TQueue : IQueueAggregator
    {
        [Test]
        public void WriteScope_OnlyIncreaseScopeIsLogged_IgnoreScope()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseScope();
            }

            writerMock.VerifyWriteLogIsNotCalled();
        }
        [Test]
        public void WriteMethod_OnlyIncreaseMethodIsLogged_IgnoreScope()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseMethod();
            }

            writerMock.VerifyWriteLogIsNotCalled();
        }
        
        [Test]
        public void WriteScope_FullScope_DecreaseScopeCauseWriting()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseScope();
                queue.WriteLog(LogLevel.Error);
                queue.WriteDecreaseScope();
                writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
            }
        }
        
        [Test]
        public void WriteMethod_FullMethod_DecreaseScopeCauseWriting()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseMethod();
                queue.WriteLog(LogLevel.Error);
                queue.WriteDecreaseScope();
                writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
            }
        }
        
        [Test]
        public void WriteScope_DecreaseScopeIsMissing_MessageIsWrittenDuringDisposing()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseScope();
                queue.WriteLog(LogLevel.Error);
            }
            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }
        
        [Test]
        public void WriteMethod_DecreaseScopeIsMissing_MessageIsWrittenDuringDisposing()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseMethod();
                queue.WriteLog(LogLevel.Error);
            }
            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }
        
        protected abstract TQueue CreateQueue(ILogWriter writer);
    }
}