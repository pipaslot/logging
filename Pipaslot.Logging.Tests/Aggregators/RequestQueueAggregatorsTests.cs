﻿using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.States;
using Pipaslot.Logging.Tests.Mocks;

namespace Pipaslot.Logging.Tests.Aggregators
{
    [TestFixture]
    class RequestQueueAggregatorsTests : BaseQueueAggregatorsTests<RequestQueueAggregator>
    {
        [Test]
        public void SingleLogFromCli_Ignore()
        {
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteLog(LogLevel.Critical, Constants.CliTraceIdentifierPrefix + "trace");
            }

            writerMock.VerifyWriteLogIsNotCalled();
        }


        [Test]
        public void CLILogsCombinedWithRequest_IgnoreCLI()
        {
            var requestId = "trace";
            var cliId = Constants.CliTraceIdentifierPrefix + "trace";
            var writerMock = new LogWritterMock();
            using (var queue = CreateQueue(writerMock.Object)){
                queue.WriteIncreaseMethod(cliId);
                queue.WriteLog(LogLevel.Critical, cliId);

                queue.WriteIncreaseScope(requestId);
                queue.WriteLog(LogLevel.Critical, requestId);
                queue.WriteDecreaseScope();

                queue.WriteIncreaseScope(cliId);
                queue.WriteLog(LogLevel.Critical, cliId);
            }

            writerMock.VerifyWriteLogIsCalledOnceWithLogCountEqualTo(2);
        }

        protected override RequestQueueAggregator CreateQueue(ILogWriter writer)
        {
            var optionsMock = new PipaslotLoggerOptionsMock();
            return new RequestQueueAggregator(writer, optionsMock.Object);
        }
    }
}