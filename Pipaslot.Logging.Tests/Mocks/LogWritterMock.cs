using Moq;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Tests.Mocks
{
    public class LogWritterMock : Mock<ILogWriter>
    {
        public void VerifyWriteLogIsCalledXTimesWithLogCountEqualTo(int times, int count)
        {
            Verify(w => w.WriteLog(It.Is<Queue>(scope => scope.Logs.Count == count)), Times.Exactly(times));
        }
        public void VerifyWriteLogIsCalledOnceWithLogCountEqualTo(int count)
        {
            Verify(w => w.WriteLog(It.Is<Queue>(scope => scope.Logs.Count == count)), Times.Once);
        }

        public void VerifyWriteLogIsNotCalled()
        {
            Verify(w => w.WriteLog(It.IsAny<Queue>()), Times.Never);
        }

        public void VerifyWriteLogIsCalledOnce()
        {
            Verify(w => w.WriteLog(It.IsAny<Queue>()), Times.Once);
        }
    }
}