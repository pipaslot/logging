using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators.Abstraction
{
    internal abstract class IncludingWrappingScopeInResultTests<TQueue>
        where TQueue : IQueueAggregator
    {
        protected abstract string TraceId { get; }

        [Test]
        public void WriteScope_FullScope_DecreaseScopeCauseWritingWithWrappingScopeInResult()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseScope(TraceId);
                queue.WriteLog(LogLevel.Critical, TraceId);
                queue.WriteDecreaseScope(TraceId);
                writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
            }
        }

        [Test]
        public void WriteMethod_FullMethod_DecreaseScopeCauseWritingWithWrappingMethodInResult()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseMethod(TraceId);
                queue.WriteLog(LogLevel.Critical, TraceId);
                queue.WriteDecreaseScope(TraceId);
                writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
            }
        }

        [Test]
        public void WriteScope_DecreaseScopeIsMissing_MessageIsWrittenDuringDisposingWithWrappingScopeInResult()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseScope(TraceId);
                queue.WriteLog(LogLevel.Critical, TraceId);
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }

        [Test]
        public void WriteMethod_DecreaseScopeIsMissing_MessageIsWrittenDuringDisposingWithWrappingMethodInResult()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseMethod(TraceId);
                queue.WriteLog(LogLevel.Critical, TraceId);
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }

        protected abstract TQueue CreateQueue(ILogWriter writer);
    }
}