using System;
using Serilog.Configuration;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using StackExchange.Redis;

namespace Serilog.Sinks.Redis.List
{
    public static class RedisLoggerConfigurationExt
    {
        private static bool _abortOnConnectFail = false;
        public static LoggerConfiguration RedisList(this LoggerSinkConfiguration loggerSinkConfiguration,
            string redisUris, string keyName,
            TimeSpan? period = null, int batchSizeLimit = RedisListLPushSink.DefaultBatchPostingLimit,
            ITextFormatter formatter = null)
        {
            var defaultedPeriod = period ?? RedisListLPushSink.DefaultPeriod;
            IConnectionMultiplexer redis = Connect(redisUris);
            var defaultFormatter = formatter ?? new RenderedCompactJsonFormatter();
            var redisClient = new RedisListLPushClient(redis, keyName, defaultFormatter);

            return
                loggerSinkConfiguration.Sink(
                    new RedisListLPushSink(redisClient, defaultedPeriod, batchSizeLimit)
                );
        }

        private static IConnectionMultiplexer Connect(string redisUris)
        {
            try
            {
                var config = ConfigurationOptions.Parse(redisUris);
                config.AbortOnConnectFail = _abortOnConnectFail;
                var multiplexer = ConnectionMultiplexer.Connect(config);

                return multiplexer;
            }
            catch (Exception)
            {
                return new NoConnectionMultiplexer(redisUris);
            }
            
        }

        //public static LoggerConfiguration RedisRollingList(this LoggerSinkConfiguration loggerSinkConfiguration, string redisUris, string keyFormat,
        //    TimeSpan? period = null, int batchSizeLimit = RedisListLPushSink.DefaultBatchPostingLimit, ITextFormatter formatter = null)
        //{
        //    return null;
        //}
    }

}
