using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class RequestQueueAggregatorsTests
    {
        [Test]
        public void SingleLogFromCli_Ignore()
        {
            var writerMock = new LogWritterMock();
            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = null
            };
            var logger = CreateLogger(writerMock.Object, httpContextAccessor);
            logger.LogCritical("message");

            writerMock.VerifyWriteLogIsNotCalled();
        }

        [Test]
        public void CLILogsCombinedWithRequest_IgnoreCLI()
        {
            var writerMock = new LogWritterMock();
            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = null
            };
            var logger = CreateLogger(writerMock.Object, httpContextAccessor);
            var requestContext = new DefaultHttpContext();

            //CLI 1
            using (logger.BeginMethod())
            {
                logger.LogCritical("message");
                //Request 1
                httpContextAccessor.HttpContext = requestContext;
                using (logger.BeginMethod())
                {
                    logger.LogCritical("message");
                    // Cli 2
                    httpContextAccessor.HttpContext = null;
                    using (logger.BeginMethod())
                    {
                        logger.LogCritical("message");
                    }
                    
                    //Back to Request 1
                    httpContextAccessor.HttpContext = requestContext;
                }
                
                // Back to Cli 1
                httpContextAccessor.HttpContext = null;
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }

        private PipaslotLogger CreateLogger(ILogWriter writer, IHttpContextAccessor httpContextAccessor)
        {
            return TestLoggerFactory.CreateLogger(IQueueAggregatorExtensions.Category, httpContextAccessor, 
                (o) => new RequestQueueAggregator(writer, o));
        }
    }
}