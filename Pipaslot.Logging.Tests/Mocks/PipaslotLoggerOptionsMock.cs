using Microsoft.Extensions.Options;
using Moq;

namespace Pipaslot.Logging.Tests.Mocks
{
    public class PipaslotLoggerOptionsMock : Mock<IOptions<PipaslotLoggerOptions>>
    {
        public PipaslotLoggerOptionsMock()
        {
            Setup(o => o.Value).Returns(new PipaslotLoggerOptions());
        }
    }
}