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
                queue.WriteScopeChange("trace", "category", new IncreaseScopeState("method"));
            }

            writerMock.VerifyWriteLogIsNotCalled();
        }
        
        [Test]
        public void WriteScope_FullScope_DecreaseScopeCauseWriting()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteScopeChange("trace", "category", new IncreaseScopeState("method"));
                queue.WriteLog("trace", "category", LogLevel.Error, "message", new { });
                queue.WriteScopeChange("trace", "category", new DecreaseScopeState());
                writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
            }
        }
        
        [Test]
        public void WriteScope_DecreaseScopeIsMissing_MessageIsWrittenDuringDisposing()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteScopeChange("trace", "category", new IncreaseScopeState("method"));
                queue.WriteLog("trace", "category", LogLevel.Error, "message", new { });
            }
            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }
        
        protected abstract TQueue CreateQueue(ILogWriter writer);
    }
}