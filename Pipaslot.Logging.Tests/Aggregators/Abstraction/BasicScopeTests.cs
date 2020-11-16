using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators.Abstraction
{
    internal abstract class BasicScopeTests<TQueue> where TQueue : IQueueAggregator
    {
        [Test]
        public void WriteScope_OnlyIncreaseScopeIsLogged_IgnoreScope()
        {
            var writerMock = new LogWritterMock();
            var logger = CreateLogger(writerMock.Object);
            logger.BeginScope(null);
            writerMock.VerifyWriteLogIsNotCalled();
        }

        [Test]
        public void WriteMethod_OnlyIncreaseMethodIsLogged_IgnoreScope()
        {
            var writerMock = new LogWritterMock();
            var logger = CreateLogger(writerMock.Object);
            logger.BeginMethod();

            writerMock.VerifyWriteLogIsNotCalled();
        }

        protected abstract ILogger CreateLogger(ILogWriter writer);
    }
}