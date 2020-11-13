using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class ProcessQueueAggregatorsTests
    {
        [Test]
        public void SingleLogFromRequest_Ignore()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteLog(LogLevel.Critical, "trace");
            }

            writerMock.VerifyWriteLogIsNotCalled();
        }


        [Test]
        public void CLILogsCombinedWithRequest_IgnoreCLI()
        {
            var requestId = "trace";
            var cliId = Constants.CliTraceIdentifierPrefix + "trace";
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseMethod(requestId);
                queue.WriteLog(LogLevel.Critical, requestId);

                queue.WriteIncreaseScope(cliId);
                queue.WriteLog(LogLevel.Critical, cliId);
                queue.WriteDecreaseScope();

                queue.WriteIncreaseScope(requestId);
                queue.WriteLog(LogLevel.Critical, requestId);
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }

        private ProcessQueueAggregator CreateQueue(ILogWriter writer)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new ProcessQueueAggregator(writer, optionsMock.Object);
        }
    }
}