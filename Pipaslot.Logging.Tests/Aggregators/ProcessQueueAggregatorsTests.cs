using Microsoft.AspNetCore.Http;
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
            var httpContextAccessor = new HttpContextAccessor()
            {
                HttpContext = new DefaultHttpContext()
            };
            var logger = CreateLogger(writerMock.Object, httpContextAccessor);
            logger.LogCritical("message");

            writerMock.VerifyWriteLogIsNotCalled();
        }
        
        [Test]
        public void CLILogsCombinedWithRequest_IgnoreRequest()
        {
            var writerMock = new LogWritterMock();
            
            var requestContext = new DefaultHttpContext();
            var httpContextAccessor = new HttpContextAccessor();
            var logger = CreateLogger(writerMock.Object, httpContextAccessor);

            //Request
            httpContextAccessor.HttpContext = requestContext;
            using (logger.BeginMethod())
            {
                logger.LogCritical("message");
                // CLI
                httpContextAccessor.HttpContext = null;
                using (logger.BeginMethod())
                {
                    logger.LogCritical("message");
                    //Request
                    httpContextAccessor.HttpContext = requestContext;
                    using (logger.BeginMethod())
                    {
                        logger.LogCritical("message");
                    }
                    // CLI
                    httpContextAccessor.HttpContext = null;
                }
                
                //Request
                httpContextAccessor.HttpContext = requestContext;
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }

        private PipaslotLogger CreateLogger(ILogWriter writer, IHttpContextAccessor httpContextAccessor)
        {
            return TestLoggerFactory.CreateLogger(IQueueAggregatorExtensions.Category,httpContextAccessor,
                (o) => new ProcessQueueAggregator(writer, o));
        }
    }
}