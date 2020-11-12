using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Pipaslot.Logging.Queues;
using Pipaslot.Logging.Records;
using Pipaslot.Logging.States;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Queues
{
    [TestFixture]
    class SendQueueTests : BaseQueueTestsWithMinimalLogLevel<SendQueue>
    {
        protected override SendQueue CreateQueue(ILogWriter writer, LogLevel level)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new SendQueue(optionsMock.Object, level, writer);
        }

        protected override SendQueue CreateQueue(ILogWriter writer)
        {
            return CreateQueue(writer, LogLevel.Error);
        }
    }
}