using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Serilog.Events;
using StackExchange.Redis;

namespace Serilog.Sinks.Redis.Sinks
{
    public class BasicRedisListSink : PeriodicBatching.PeriodicBatchingSink
    {
        public const int DefaultBatchPostingLimit = 50;
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);

        private readonly IConnectionMultiplexer _redis;

        private readonly string _keyName;
        private readonly IFormatProvider _formatter;

        public BasicRedisListSink(IConnectionMultiplexer redis, string keyName, TimeSpan period, int batchSizeLimit = DefaultBatchPostingLimit, IFormatProvider formatter = null) : base(batchSizeLimit, period)
        {
            if (redis == null)
                throw new ArgumentNullException(nameof(redis));

            if (string.IsNullOrEmpty(keyName))
                throw new ArgumentNullException(nameof(keyName));

            _redis = redis;
            _keyName = keyName;
            _formatter = formatter;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _redis.Dispose();
        }

        protected override bool CanInclude(LogEvent evt)
        {
            if (evt == null)
                return false;

            return base.CanInclude(evt);
        }

        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            var db = _redis.GetDatabase();
            db.ListLeftPush(_keyName, TransformLogValues(events));
        }

        private RedisValue[] TransformLogValues(IEnumerable<LogEvent> events)
        {
            return events
                .Select(ev => (RedisValue) JsonConvert.SerializeObject(new RedisLogEvent(ev, ev.RenderMessage(_formatter))))
                .ToArray();
        }
    }
}
