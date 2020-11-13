using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators.Abstraction
{
    internal abstract class BaseTests<TQueue> where TQueue : IQueueAggregator
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

        protected abstract TQueue CreateQueue(ILogWriter writer);
    }
}