using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Parsing;
using Serilog.Sinks.Redis.List;
using Xunit;

namespace Serilog.Sinks.Redis.Tests
{
    public class RedisListLPushClientTests
    {
        [Fact]
        public void Send_ErrorEvent_SendLevelAsError()
        {
            var sut = CreateSut();
            var events = new List<LogEvent>
            {
                new LogEvent(DateTimeOffset.Now, LogEventLevel.Error, null,
                    new MessageTemplate(string.Empty, new List<MessageTemplateToken>()), new List<LogEventProperty>())
            };

            sut.Send(events);
            var result = sut.DataValues.First().ToString();
            Assert.Contains("\"@l\":\"Error\"", result);
        }

        private static TestableRedisListLPushClient CreateSut()
        {
            var redis = new NoConnectionMultiplexer("");
            var sut = new TestableRedisListLPushClient(redis, "test", new RenderedCompactJsonFormatter());
            return sut;
        }
    }
}
