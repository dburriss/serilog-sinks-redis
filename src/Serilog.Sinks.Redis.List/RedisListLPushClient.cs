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
        protected readonly IConnectionMultiplexer Redis;
        protected readonly string KeyName;
        protected readonly ITextFormatter Formatter;

        public RedisListLPushClient(IConnectionMultiplexer redis, string keyName, ITextFormatter defaultFormatter)
        {
            Redis = redis;
            KeyName = keyName;
            Formatter = defaultFormatter;
        }

        public virtual void Send(IEnumerable<LogEvent> events)
        {
            if (Redis.IsConnected)
            {
                var db = Redis.GetDatabase();
                db.ListLeftPush(KeyName, TransformLogValues(events).ToArray<RedisValue>());
            }
        }

        protected virtual IEnumerable<RedisValue> TransformLogValues(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                using (var sw = new StringWriter())
                {
                    Formatter.Format(logEvent, sw);
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
                Redis.Dispose();

            _disposed = true;
        }
    }

}