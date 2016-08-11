using System;
using Serilog.Configuration;
using Serilog.Sinks.Redis.Sinks;
using StackExchange.Redis;

namespace Serilog.Sinks.Redis
{
    public static class RedisLoggerConfigurationExt
    {
        public static LoggerConfiguration Redis(this LoggerSinkConfiguration loggerSinkConfiguration,string redisUris, string keyName, 
            TimeSpan? period = null, int batchSizeLimit = BasicRedisListSink.DefaultBatchPostingLimit, IFormatProvider formatter = null)
        {
            var defaultedPeriod = period ?? BasicRedisListSink.DefaultPeriod;
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisUris);

            return
                loggerSinkConfiguration.Sink(
                    new BasicRedisListSink(redis, keyName, defaultedPeriod, batchSizeLimit, formatter)
                    );
        }
    }
}
