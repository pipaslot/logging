using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class FlatQueueAggregatorsBaseTests : BaseTests<FlatQueueAggregator>
    {
        [Test]
        public void WriteScope_FullScope_DecreaseScopeCauseWriting()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseScope();
                queue.WriteLog(LogLevel.Critical);
                queue.WriteDecreaseScope();
                writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(1);
            }
        }

        [Test]
        public void WriteMethod_FullMethod_DecreaseScopeCauseWriting()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseMethod();
                queue.WriteLog(LogLevel.Critical);
                queue.WriteDecreaseScope();
                writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(1);
            }
        }

        [Test]
        public void WriteScope_DecreaseScopeIsMissing_MessageIsWrittenDuringDisposing()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseScope();
                queue.WriteLog(LogLevel.Critical);
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(1);
        }

        [Test]
        public void WriteMethod_DecreaseScopeIsMissing_MessageIsWrittenDuringDisposing()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseMethod();
                queue.WriteLog(LogLevel.Critical);
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(1);
        }

        protected override FlatQueueAggregator CreateQueue(ILogWriter writer)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new FlatQueueAggregator(writer, LogLevel.Error, optionsMock.Object);
        }
    }
}