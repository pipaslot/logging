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
        public void WriteLogOnly_LogOnlyEqualOrHigherPriorities_WriteEverySingle(LogLevel minimalLevel, int expected)
        {
            var writerMock = new LogWritterMock();
            var logger = CreateLogger(writerMock.Object, minimalLevel);
            logger.LogCritical("message");
            logger.LogError("message");
            logger.LogWarning("message");
            logger.LogInformation("message");
            logger.LogDebug("message");
            logger.LogTrace("message");

            writerMock.VerifyWriteLogIsCalledXTimesWithLogCountEqualTo(expected, 1);
        }

        [TestCase(LogLevel.Critical, 1)]
        [TestCase(LogLevel.Error, 2)]
        [TestCase(LogLevel.Warning, 3)]
        [TestCase(LogLevel.Information, 4)]
        [TestCase(LogLevel.Debug, 5)]
        [TestCase(LogLevel.Trace, 6)]
        public void WriteLogsInsideScope_LogOnlyEqualOrHigherPriorities_IgnoreScopeAndWriteAllSeparatedly(LogLevel minimalLevel, int expected)
        {
            var writerMock = new LogWritterMock();
            var logger = CreateLogger(writerMock.Object, minimalLevel);
            using (logger.BeginMethod())
            {
                logger.LogCritical("message");
                logger.LogError("message");
                logger.LogWarning("message");
                logger.LogInformation("message");
                logger.LogDebug("message");
                logger.LogTrace("message");
            }
            writerMock.VerifyWriteLogIsCalledXTimesWithLogCountEqualTo(expected, 1);
        }

        [Test]
        public void WriteScope_FullScope_DecreaseScopeCauseWriting()
        {
            var writerMock = new LogWritterMock();
            var logger = CreateLogger(writerMock.Object);

            using (logger.BeginScope(null))
            {
                logger.LogCritical("message");
            }
            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(1);
        }

        [Test]
        public void WriteMethod_FullMethod_DecreaseScopeCauseWriting()
        {
            var writerMock = new LogWritterMock();
            var logger = CreateLogger(writerMock.Object);
            using (logger.BeginMethod())
            {
                logger.LogCritical("message");
            }
            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(1);
        }

        [Test]
        public void WriteScope_DecreaseScopeIsMissing_MessageIsWrittenDuringDisposing()
        {
            var writerMock = new LogWritterMock();
            using (var logger = CreateLogger(writerMock.Object))
            {
                logger.BeginScope(null);
                logger.LogCritical("message");
            }
            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(1);
        }

        [Test]
        public void WriteMethod_DecreaseScopeIsMissing_MessageIsWrittenDuringDisposing()
        {
            var writerMock = new LogWritterMock();
            using (var logger = CreateLogger(writerMock.Object))
            {
                logger.BeginMethod();
                logger.LogCritical("message");
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(1);
        }

        private PipaslotLogger CreateLogger(ILogWriter writer, LogLevel level = LogLevel.Error)
        {
            return TestLoggerFactory.CreateLogger(IQueueAggregatorExtensions.Category,
                (o) => new FlatQueueAggregator(writer, level, o));
        }
    }
}