using System;
using System.IO;
using System.Text;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using StackExchange.Redis;

namespace Serilog.Sinks.Redis.List
{
    public static class RedisLoggerConfigurationExt
    {
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
                return ConnectionMultiplexer.Connect(redisUris);
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
