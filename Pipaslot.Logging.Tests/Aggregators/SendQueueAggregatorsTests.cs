using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class SendQueueAggregatorsTests
    {
        [Test]
        public void WriteSingle_HigherLevel_OneMessageIsInserted()
        {
            var writerMock = new LogWritterMock();
            var logger = CreateLogger(writerMock.Object, LogLevel.Information);
            logger.LogError("message");

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(1);
        }

        [Test]
        public void WriteScopeAndLog_LogHasLowPriority_IgnoreScope()
        {
            var writerMock = new LogWritterMock();
            var logger = CreateLogger(writerMock.Object);
            logger.BeginScope(null);
            logger.LogInformation("message");


            writerMock.VerifyWriteLogIsNotCalled();
        }

        [Test]
        public void WriteMethodAndLog_LogHasLowPriority_IgnoreScope()
        {
            var writerMock = new LogWritterMock();
            var logger = CreateLogger(writerMock.Object);
            logger.BeginMethod();
            logger.LogInformation("message");

            writerMock.VerifyWriteLogIsNotCalled();
        }

        [Test]
        public void WriteOnlyInfoInNestedScopes_ShouldIgnore()
        {
            var writerMock = new LogWritterMock();
            var logger = CreateLogger(writerMock.Object);
            using (logger.BeginScope(null))
            {
                logger.LogInformation("message");
                using (logger.BeginScope(null))
                {
                    logger.LogInformation("message");
                }
                logger.LogInformation("message");
            }
            writerMock.VerifyWriteLogIsNotCalled();

        }

        [Test]
        public void WriteErrorInNestedScope_ShouldWrite()
        {
            var writerMock = new LogWritterMock();
            var logger = CreateLogger(writerMock.Object);
            using (logger.BeginScope(null))
            {
                logger.LogInformation("message");
                using (logger.BeginScope(null))
                {
                    logger.LogError("message");
                }
                logger.LogInformation("message");
            }
            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(6);

        }

        protected PipaslotLogger CreateLogger(ILogWriter writer, LogLevel level = LogLevel.Error)
        {
            return TestLoggerFactory.CreateLogger(IQueueAggregatorExtensions.Category,
                (o) => new SendQueueAggregator(o, level, writer));
        }
    }
}