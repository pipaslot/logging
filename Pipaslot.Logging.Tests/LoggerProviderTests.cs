using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests
{
    public class LoggerProviderTests
    {
        [TestCase("Category")]
        [TestCase("Category.With.Namespace")]
        public void CreateLogger_DuplicateCategoryName_ReturnsTheSameLogger(string category)
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var optionsMock = new PipaslotLoggerOptionsMock();
            var sut = new PipaslotLoggerProvider(httpContextAccessorMock.Object, new Pipe[0], optionsMock.Object);

            var logger1 = sut.CreateLogger(category);
            var logger2 = sut.CreateLogger(category);
            Assert.AreSame(logger1, logger2);
        }
    }
}