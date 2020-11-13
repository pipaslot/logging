using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Tests.Aggregators.Abstraction;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    internal class ProcessQueueAggregatorsIncludingWrappingScopeInResultTests : IncludingWrappingScopeInResultTests<ProcessQueueAggregator>
    {
        protected override string TraceId => Constants.CliTraceIdentifierPrefix + "trace";

        protected override ProcessQueueAggregator CreateQueue(ILogWriter writer)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new ProcessQueueAggregator(writer, optionsMock.Object);
        }
    }
}