using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.Redis.Core;
using StackExchange.Redis;

namespace Serilog.Sinks.Redis.List
{
    public class RedisListLPushClient : IRedisClient
    {
        protected readonly IConnectionMultiplexer _redis;
        protected readonly string _keyName;
        protected readonly ITextFormatter _formatter;

        public RedisListLPushClient(IConnectionMultiplexer redis, string keyName, ITextFormatter defaultFormatter)
        {
            _redis = redis;
            _keyName = keyName;
            _formatter = defaultFormatter;
        }

        public virtual void Send(IEnumerable<LogEvent> events)
        {
            var db = _redis.GetDatabase();
            db.ListLeftPush(_keyName, TransformLogValues(events).ToArray<RedisValue>());
        }

        protected virtual IEnumerable<RedisValue> TransformLogValues(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                using (var sw = new StringWriter())
                {
                    _formatter.Format(logEvent, sw);
                    yield return sw.ToString();
                }
            }
        }

        public void Dispose()
        {
            Dispose(false);
        }

        bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _redis.Dispose();

            _disposed = true;
        }
    }

}