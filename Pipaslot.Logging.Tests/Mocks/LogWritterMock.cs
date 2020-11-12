using Moq;
using Pipaslot.Logging.Records;

namespace Pipaslot.Logging.Tests.Mocks
{
    public class LogWritterMock : Mock<ILogWriter>
    {
        public void VerifyWriteLogIsCalledOnceWithLogCountEqualTo(int count)
        {
            Verify(w => w.WriteLog(It.Is<LogScope>(scope => scope.Logs.Count == count)), Times.Once);
        }
        
        public void VerifyWriteLogIsNotCalled()
        {
            Verify(w => w.WriteLog(It.IsAny<LogScope>()), Times.Never);
        }
    }
}