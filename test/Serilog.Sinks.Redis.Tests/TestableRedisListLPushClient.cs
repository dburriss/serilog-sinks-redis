using System.Collections.Generic;
using System.Linq;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.Redis.List;
using StackExchange.Redis;

namespace Serilog.Sinks.Redis.Tests
{
    public class TestableRedisListLPushClient : RedisListLPushClient
    {
        public bool SendCalled { get; set; }
        public IEnumerable<RedisValue> DataValues { get; set; }
        public TestableRedisListLPushClient(IConnectionMultiplexer redis, string keyName, ITextFormatter defaultFormatter) : base(redis, keyName, defaultFormatter)
        {
        }

        public override void Send(IEnumerable<LogEvent> events)
        {
            var logEvents = events as IList<LogEvent> ?? events.ToList();
            base.Send(logEvents);
            DataValues = TransformLogValues(logEvents);
            SendCalled = true;
        }


    }
}