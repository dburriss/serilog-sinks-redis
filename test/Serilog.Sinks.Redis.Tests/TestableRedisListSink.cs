using System;
using System.Collections.Generic;
using Serilog.Events;
using Serilog.Sinks.Redis.Core;
using Serilog.Sinks.Redis.List;

namespace Serilog.Sinks.Redis.Tests
{
    public class TestableRedisListSink : RedisListLPushSink
    {
        public bool CalledEmit { get; private set; }

        public TestableRedisListSink(IRedisClient client, TimeSpan period, int batchSizeLimit = DefaultBatchPostingLimit) : base(client, period, batchSizeLimit)
        {}

        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            CalledEmit = true;
            base.EmitBatch(events);
        }
    }
}
