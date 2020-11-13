using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class FlatQueueAggregatorsTests
    {
        [TestCase(LogLevel.Critical, 1)]
        [TestCase(LogLevel.Error, 2)]
        [TestCase(LogLevel.Warning, 3)]
        [TestCase(LogLevel.Information, 4)]
        [TestCase(LogLevel.Debug, 5)]
        [TestCase(LogLevel.Trace, 6)]
        public void WriteLogOnly_LogOnlyEqualOrHigherPriorities(LogLevel minimalLevel, int expected)
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object, minimalLevel)){
                queue.WriteLog(LogLevel.Critical);
                queue.WriteLog(LogLevel.Error);
                queue.WriteLog(LogLevel.Warning);
                queue.WriteLog(LogLevel.Information);
                queue.WriteLog(LogLevel.Debug);
                queue.WriteLog(LogLevel.Trace);
                queue.WriteLog(LogLevel.None);
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(expected);
        }

        private FlatQueueAggregator CreateQueue(ILogWriter writer, LogLevel level)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new FlatQueueAggregator(writer, level, optionsMock.Object);
        }
    }
}