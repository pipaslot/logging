using System;
using System.IO;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;

namespace Pipaslot.Logging.Tests
{
    //TODO 
    public class RequestWriterTests
    {
//        private string _directory;
//
//        [SetUp]
//        public void Setup()
//        {
//            _directory = Path.Combine(Directory.GetCurrentDirectory(), "logs", Guid.NewGuid().ToString());
//        }
//
//        [TearDown]
//        public void TearDown()
//        {
//            Directory.Delete(_directory, true);
//        }
//
//        [Test]
//        public void Test1()
//        {
//            var sut = new RequestWriter(new WriterSetting(_directory, Guid.NewGuid() + ".log"));
//            
//            sut.Write<object>("R1", "Microsoft.AspNetCore.Hosting.Internal.WebHost", LogLevel.Trace, "Request starting", null);
//            sut.Write<object>("R1", "My.App", LogLevel.Trace, "Do It", null);
//
//            Assert.Pass();
//        }
        

        /*public static ILoggerProvider CreateWithRequestWriter(string directory, LogLevel logLevel)
        {
            var settings = new WriterSetting(directory, "{Date}-requests.log")
            {
                LogLevel = logLevel
            };
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(a => a.HttpContext).Returns(new DefaultHttpContext());
            var writer = new RequestWriter(settings);
            return new LoggerProvider(httpContextAccessorMock.Object, new[] {writer});
        }*/
    }
}