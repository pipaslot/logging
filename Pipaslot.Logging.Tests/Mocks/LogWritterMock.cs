using Moq;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging.Tests.Mocks
{
    public class LogWritterMock : Mock<ILogWriter>
    {
        public void VerifyWriteLogIsCalledXTimesWithLogCountEqualTo(int times, int count)
        {
            Verify(w => w.WriteLog(It.Is<GrowingQueue>(scope => scope.Count == count)), Times.Exactly(times));
        }

        public void VerifyWriteLogIsCalledOnceWithLogCountEqualTo(int count)
        {
            Verify(w => w.WriteLog(It.Is<GrowingQueue>(scope => scope.Count == count)), Times.Once);
        }

        public void VerifyWriteLogIsNotCalled()
        {
            Verify(w => w.WriteLog(It.IsAny<GrowingQueue>()), Times.Never);
        }

        public void VerifyWriteLogIsCalledOnce()
        {
            Verify(w => w.WriteLog(It.IsAny<GrowingQueue>()), Times.Once);
        }
    }
}