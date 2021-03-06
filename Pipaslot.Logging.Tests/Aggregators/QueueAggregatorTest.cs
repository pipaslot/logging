﻿using Microsoft.AspNetCore.Http;
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
            QueueAggregator.CanCreateQueueFromScopes = true;
            _writerMock = new LogWritterMock();
        }

        private PipaslotLogger CreateLogger(string defaultTraceIdentifier = "Request123")
        {
            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
                {
                    TraceIdentifier = defaultTraceIdentifier
                }
            };
            var options = new PipaslotLoggerOptionsMock();
            var pipes = new[]
            {
                new Pipe(_writerMock.Object, new NullFilter())
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
            public IQueue Filter(IQueue queue)
            {
                return queue;
            }
        }

        #endregion

        #region ScopeBegin Only - ignored

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

        #region Missing ScopeEnd in nested scopes - Cause write

        [Test]
        public void MissingScopeEndInNestedScopes_ScopeEndIsMissing_WriteScope()
        {
            var logger = CreateLogger();
            using (logger.BeginScope(null))
            {
                logger.BeginScope(null);
                logger.Log(LogLevel.Critical, "message");
            }
            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(4);
        }

        [Test]
        public void MissingMethodEndInNestedScopes_ScopeEndIsMissing_WriteScope()
        {
            var logger = CreateLogger();
            using (logger.BeginMethod())
            {
                logger.BeginMethod();
                logger.Log(LogLevel.Critical, "message");
            }
            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(4);
        }

        #endregion

        #region ScopeEnd with record - Cause write

        [Test]
        public void ScopeEnd_FullScope_DecreaseScopeCauseWritingWithWrappingScopeInResult()
        {
            var logger = CreateLogger();
            using (logger.BeginScope(null)){
                logger.Log(LogLevel.Critical, "message");
            }

            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(3);
        }

        [Test]
        public void ScopeEnd_FullMethod_DecreaseScopeCauseWritingWithWrappingMethodInResult()
        {
            var logger = CreateLogger();
            using (logger.BeginMethod()){
                logger.Log(LogLevel.Critical, "message");
            }

            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(3);
        }

        [Test]
        public void ScopeEnd_NestedFullScopeWithRecord_DecreaseScopeCauseWritingWithWrappingScopeInResult()
        {
            var logger = CreateLogger();
            using (logger.BeginScope(null)){
                using (logger.BeginScope(null)){
                    logger.Log(LogLevel.Critical, "message");
                }
            }

            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(5);
        }

        [Test]
        public void ScopeEnd_NestedFullMethodWithRecord_DecreaseScopeCauseWritingWithWrappingMethodInResult()
        {
            var logger = CreateLogger();
            using (logger.BeginMethod()){
                using (logger.BeginMethod()){
                    logger.Log(LogLevel.Critical, "message");
                }
            }

            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(5);
        }

        #endregion

        #region ScopeEnd without record - is ignored

        [Test]
        public void ScopeEnd_FullScopeWithoutRecord_WriteIsIgnored()
        {
            var logger = CreateLogger();
            using (logger.BeginScope(null)){
            }

            _aggregator.Dispose();// Ignore event if dispose is called
            _writerMock.VerifyWriteLogIsNotCalled();
        }

        [Test]
        public void ScopeEnd_FullMethodWithoutRecord_WriteIsIgnored()
        {
            var logger = CreateLogger();
            using (logger.BeginMethod()){
            }

            _aggregator.Dispose();// Ignore event if dispose is called
            _writerMock.VerifyWriteLogIsNotCalled();
        }

        [Test]
        public void ScopeEnd_NestedFullScopeWithoutRecord_WriteIsIgnored()
        {
            var logger = CreateLogger();
            using (logger.BeginScope(null)){
                using (logger.BeginScope(null)){
                }
            }

            _aggregator.Dispose();// Ignore event if dispose is called
            _writerMock.VerifyWriteLogIsNotCalled();
        }

        [Test]
        public void ScopeEnd_NestedFullMethodWithoutRecord_WriteIsIgnored()
        {
            var logger = CreateLogger();
            using (logger.BeginMethod()){
                using (logger.BeginMethod()){
                }
            }

            _aggregator.Dispose();// Ignore event if dispose is called
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

        #region RequestLoggerMiddleware integration

        [Test]
        public void RequestLoggerMiddleware_LogScopeWithoutScopeBeginCalled_IgnoreScope()
        {
            var traceIdentifier = "Request123";
            QueueAggregator.CanCreateQueueFromScopes = false;
            var logger = CreateLogger(traceIdentifier);
            using(logger.BeginScope(null)){
                logger.Log(LogLevel.Critical, "message");
            }
            _writerMock.VerifyWriteLogIsNotCalled();
        }

        [Test]
        public void RequestLoggerMiddleware_LogScopeWithScopeBeginCalled_WriteScope()
        {
            var traceIdentifier = "Request123";
            QueueAggregator.CanCreateQueueFromScopes = false;
            var logger = CreateLogger(traceIdentifier);
            _aggregator.BeginQueue(traceIdentifier);
            using(logger.BeginScope(null)){
                logger.Log(LogLevel.Critical, "message");
            }
            _aggregator.EndQueue(traceIdentifier);
            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(3);
        }

        [Test]
        public void RequestLoggerMiddleware_LogScopeWithoutScopeBeginCalledButLoggingProcessData_WriteScope()
        {
            var traceIdentifier = Constants.CliTraceIdentifierPrefix+"123";
            QueueAggregator.CanCreateQueueFromScopes = false;
            var logger = CreateLogger(traceIdentifier);
            using(logger.BeginScope(null)){
                logger.Log(LogLevel.Critical, "message");
            }
            _writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(3);
        }

        #endregion
    }
}