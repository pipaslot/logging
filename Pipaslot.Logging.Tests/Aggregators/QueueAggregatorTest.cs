using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Filters;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    public class QueueAggregatorTest
    {
        #region Setup

        private LogWritterMock _writerMock;
        private QueueAggregator _aggregator;

        [SetUp]
        public void Setup()
        {
            _writerMock = new LogWritterMock();
        }

        private PipaslotLogger CreateLogger()
        {
            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
            };
            var options = new PipaslotLoggerOptionsMock();
            var pipes = new[]
            {
                new Pipe(_writerMock.Object, new NullFilter()),
            };
            _aggregator = new QueueAggregator(pipes, options.Object);
            return new PipaslotLogger(httpContextAccessor, _aggregator, "category");
        }

        private class HttpContextAccessor : IHttpContextAccessor
        {
            public HttpContext HttpContext { get; set; }
        }

        private class NullFilter : IQueueFilter
        {
            public Queue Filter(Queue queue)
            {
                return queue;
            }
        }

        #endregion

        #region Scope Only - ignored

        [Test]
        public void ScopeOnly_OnlyIncreaseScopeIsLogged_IgnoreScope()
        {
            var logger = CreateLogger();
            logger.BeginScope(null);
            _writerMock.VerifyWriteLogIsNotCalled();
        }

        [Test]
        public void ScopeOnly_OnlyIncreaseMethodIsLogged_IgnoreScope()
        {
            var logger = CreateLogger();
            logger.BeginMethod();

            _writerMock.VerifyWriteLogIsNotCalled();
        }

        #endregion

        #region ScopeEnd with record - Cause write

        [Test]
        public void ScopeEnd_FullScope_DecreaseScopeCauseWritingWithWrappingScopeInResult()
        {
            var logger = CreateLogger();
            using (logger.BeginScope(null))
            {
                logger.Log(LogLevel.Critical, "message");
            }
            _aggregator.Dispose();
            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(3);
        }

        [Test]
        public void ScopeEnd_FullMethod_DecreaseScopeCauseWritingWithWrappingMethodInResult()
        {
            var logger = CreateLogger();
            using (logger.BeginMethod())
            {
                logger.Log(LogLevel.Critical, "message");
            }
            _aggregator.Dispose();
            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(3);
        }

        [Test]
        public void ScopeEnd_NestedFullScopeWithRecord_DecreaseScopeCauseWritingWithWrappingScopeInResult()
        {
            var logger = CreateLogger();
            using (logger.BeginScope(null))
            {
                using (logger.BeginScope(null))
                {
                    logger.Log(LogLevel.Critical, "message");
                }
            }
            _aggregator.Dispose();
            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(5);
        }

        [Test]
        public void ScopeEnd_NestedFullMethodWithRecord_DecreaseScopeCauseWritingWithWrappingMethodInResult()
        {
            var logger = CreateLogger();
            using (logger.BeginMethod())
            {
                using (logger.BeginMethod())
                {
                    logger.Log(LogLevel.Critical, "message");
                }
            }
            _aggregator.Dispose();
            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(5);
        }

        #endregion

        #region ScopeEnd without record - is ignored

        [Test]
        public void ScopeEnd_FullScopeWithoutRecord_WriteIsIgnored()
        {
            var logger = CreateLogger();
            using (logger.BeginScope(null))
            {
            }
            _aggregator.Dispose();
            _writerMock.VerifyWriteLogIsNotCalled();
        }

        [Test]
        public void ScopeEnd_FullMethodWithoutRecord_WriteIsIgnored()
        {
            var logger = CreateLogger();
            using (logger.BeginMethod())
            {
            }
            _aggregator.Dispose();
            _writerMock.VerifyWriteLogIsNotCalled();
        }

        [Test]
        public void ScopeEnd_NestedFullScopeWithoutRecord_WriteIsIgnored()
        {
            var logger = CreateLogger();
            using (logger.BeginScope(null))
            {
                using (logger.BeginScope(null))
                {
                }
            }
            _aggregator.Dispose();
            _writerMock.VerifyWriteLogIsNotCalled();
        }

        [Test]
        public void ScopeEnd_NestedFullMethodWithoutRecord_WriteIsIgnored()
        {
            var logger = CreateLogger();
            using (logger.BeginMethod())
            {
                using (logger.BeginMethod())
                {
                }
            }
            _aggregator.Dispose();
            _writerMock.VerifyWriteLogIsNotCalled();
        }

        #endregion
        
        #region Disposing

        [Test]
        public void Dispose_DecreaseScopeIsMissing_MessageIsWrittenDuringDisposingWithWrappingScopeInResult()
        {
            var logger = CreateLogger();
            logger.BeginScope(null);
            logger.Log(LogLevel.Critical, "message");

            _aggregator.Dispose();
            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }

        [Test]
        public void Dispose_DecreaseMethodIsMissing_MessageIsWrittenDuringDisposingWithWrappingMethodInResult()
        {
            var logger = CreateLogger();
            logger.BeginMethod();
            logger.Log(LogLevel.Critical, "message");

            _aggregator.Dispose();
            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }

        #endregion

    }
}
