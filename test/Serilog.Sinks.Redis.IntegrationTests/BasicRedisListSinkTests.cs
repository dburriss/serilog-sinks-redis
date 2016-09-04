using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Parsing;
using Serilog.Sinks.Redis.Core;
using Serilog.Sinks.Redis.List;
using StackExchange.Redis;
using Xunit;

namespace Serilog.Sinks.Redis.IntegrationTests
{
    public class RedisListLPushSinkTests
    {
        const string key = "ConsoleWithLogging";

        public RedisListLPushSinkTests()
        {
            var db = GetDb();
            db.KeyDelete(key);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void VerifyUp()
        {
            IConnectionMultiplexer redis = Connect();
            Assert.True(redis.IsConnected);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void VerifyDb()
        {
            IDatabase db = GetDb();
            Assert.NotNull(db);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Emit_ValidLogEvents_SendsEventToServer()
        {
            
            var db = GetDb();
            var ev = CreateLogItem();

            var sut = new RedisListLPushSink(Client(), TimeSpan.FromMilliseconds(1));

            sut.Emit(ev);
            WaitForBatchToComplete();
            var value = db.ListRightPop(key);
            
            Assert.True(value.HasValue);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Emit_ValidLogEvents_ContainsEvent()
        {

            var db = GetDb();
            var ev = CreateLogItem();

            var sut = new RedisListLPushSink(Client(), TimeSpan.FromMilliseconds(1));

            sut.Emit(ev);
            WaitForBatchToComplete();
            var length = db.ListLength(key);
            var list = db.ListRange(key, 0, length);

            Assert.True(list.Length == 1);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Emit_100ValidEvents_ContainsEvent()
        {

            var db = GetDb();
            var sut = new RedisListLPushSink(Client(), TimeSpan.FromMilliseconds(1));
            Enumerable.Range(0, 100).ToList().ForEach(i =>
            {
                var ev = CreateLogItem();
                sut.Emit(ev);
            });
            
            WaitForBatchToComplete(100);
            var length = db.ListLength(key);
            var list = db.ListRange(key, 0, length);

            Assert.True(list.Length == 100);
        }

        private static LogEvent CreateLogItem()
        {
            return new LogEvent(DateTimeOffset.UtcNow,
                LogEventLevel.Information,
                new Exception("TEST"),
                new MessageTemplate("This is a test on {Date}", new List<MessageTemplateToken>() { new TextToken("Date") }),
                new List<LogEventProperty>());
        }

        private static void WaitForBatchToComplete(int milliseconds = 50)
        {
            Thread.Sleep(milliseconds);
        }

        private IDatabase GetDb()
        {
            IConnectionMultiplexer redis = Connect();
            var db = redis.GetDatabase();
            return db;
        }

        private static ConnectionMultiplexer Connect()
        {
            return ConnectionMultiplexer.Connect("localhost:6379");
        }

        private static IRedisClient Client()
        {
            return new RedisListLPushClient(Connect(), key, new CompactJsonFormatter());
        }

    }
}
