using System;
using System.Collections.Generic;
using System.Threading;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Redis.Core;
using Telerik.JustMock;
using Xunit;

namespace Serilog.Sinks.Redis.Tests
{
    public class RedisListLPushSinkTests
    {
        [Fact]
        public void Emit_LessThanBatchLimitOfEvents_DoesNotEmitBatch()
        {
            var batchLimit = 50;
            var client = Mock.Create<IRedisClient>();

            var period = TimeSpan.FromSeconds(10);
            var sut = new TestableRedisListSink(client, period, batchLimit);
            var ev = CreateLogItem();
            sut.Emit(ev);
            Assert.False(sut.CalledEmit);
        }

        [Fact]
        public void Emit_BatchLimitOfEvents_EmitsBatch()
        {
            var batchLimit = 50;
            var client = Mock.Create<IRedisClient>();

            var period = TimeSpan.FromSeconds(10);
            var sut = new TestableRedisListSink(client, period, batchLimit);
            var ev = CreateLogItem();
            for (var i = 0; i <= batchLimit; i++)
            {
                sut.Emit(ev);
            }
            SpinWait.SpinUntil(() => sut.CalledEmit, TimeSpan.FromSeconds(5));
            Assert.True(sut.CalledEmit);
        }
        

        private static LogEvent CreateLogItem()
        {
            return new LogEvent(DateTimeOffset.MinValue,
                LogEventLevel.Information,
                new Exception("TEST"),
                new MessageTemplate(
                    "This is a test on {Date}", 
                    new List<MessageTemplateToken>() { new TextToken("Date") }
                    ),
                new List<LogEventProperty>());
        }
    }
}
