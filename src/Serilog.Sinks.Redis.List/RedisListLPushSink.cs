// https://github.com/serilog/serilog/wiki/Writing-Log-Events
// https://github.com/serilog/serilog/wiki/Formatting-Output

using System;
using System.Collections.Generic;
using Serilog.Events;
using Serilog.Sinks.Redis.Core;

namespace Serilog.Sinks.Redis.List
{
    public class RedisListLPushSink : PeriodicBatching.PeriodicBatchingSink
    {
        public const int DefaultBatchPostingLimit = 50;
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);

        private readonly IRedisClient _redisListClient;

        public RedisListLPushSink(IRedisClient client, TimeSpan period, int batchSizeLimit = DefaultBatchPostingLimit)
            : base(batchSizeLimit, period)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            _redisListClient = client;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _redisListClient.Dispose();
        }

        protected override bool CanInclude(LogEvent evt)
        {
            if (evt == null)
                return false;

            return base.CanInclude(evt);
        }

        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            _redisListClient.Send(events);
        }
    }
}
