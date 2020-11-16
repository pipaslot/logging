using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators.Abstraction
{
    internal abstract class IncludingWrappingScopeInResultTests
    {
        protected abstract string TraceId { get; }

        [Test]
        public void WriteScope_FullScope_DecreaseScopeCauseWritingWithWrappingScopeInResult()
        {
            var writerMock = new LogWritterMock();
            using (var logger = CreateLogger(writerMock.Object))
            {
                using (logger.BeginScope(null))
                {
                    logger.Log(LogLevel.Critical, "message");
                }
            }
            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);

        }

        [Test]
        public void WriteMethod_FullMethod_DecreaseScopeCauseWritingWithWrappingMethodInResult()
        {
            var writerMock = new LogWritterMock();
            using (var logger = CreateLogger(writerMock.Object))
            {
                using (logger.BeginMethod())
                {
                    logger.Log(LogLevel.Critical, "message");
                }
            }
            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);

        }

        [Test]
        public void WriteScope_DecreaseScopeIsMissing_MessageIsWrittenDuringDisposingWithWrappingScopeInResult()
        {
            var writerMock = new LogWritterMock();
            using (var logger = CreateLogger(writerMock.Object))
            {
                logger.BeginScope(null);
                logger.Log(LogLevel.Critical, "message");
            }
            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }

        [Test]
        public void WriteMethod_DecreaseScopeIsMissing_MessageIsWrittenDuringDisposingWithWrappingMethodInResult()
        {
            var writerMock = new LogWritterMock();
            using (var logger = CreateLogger(writerMock.Object))
            {
                logger.BeginMethod();
                logger.Log(LogLevel.Critical, "message");
            }
            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }

        protected abstract PipaslotLogger CreateLogger(ILogWriter writer);
    }
}