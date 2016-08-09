using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Redis.Sinks;
using StackExchange.Redis;
using Xunit;

namespace Serilog.Sinks.Redis.IntegrationTests
{
    public class BasicRedisListSinkTests
    {
        const string key = "testKey";
        [Fact]
        public void VerifyUp()
        {
            IConnectionMultiplexer redis = Connect();
            Assert.True(redis.IsConnected);
        }

        [Fact]
        public void VerifyDb()
        {
            IDatabase db = GetDb();
            Assert.NotNull(db);
        }

        [Fact]
        public void Emit_ValidLogEvents_SendsEventToServer()
        {
            
            var db = GetDb();
            var ev = CreateLogItem();

            var sut = new BasicRedisListSink(Connect(), key, TimeSpan.FromMilliseconds(1));

            sut.Emit(ev);
            WaitForBatchToComplete();
            var value = db.ListRightPop(key);
            
            Assert.True(value.HasValue);
        }

        [Fact]
        public void Emit_ValidLogEvents_ContainsEvent()
        {

            var db = GetDb();
            var ev = CreateLogItem();

            var sut = new BasicRedisListSink(Connect(), key, TimeSpan.FromMilliseconds(1));

            sut.Emit(ev);
            WaitForBatchToComplete();
            var length = db.ListLength(key);
            var list = db.ListRange(key, 0, length);

            Assert.True(list.Length == 1);
        }

        [Fact]
        public void Emit_100ValidEvents_ContainsEvent()
        {

            var db = GetDb();
            var sut = new BasicRedisListSink(Connect(), key, TimeSpan.FromMilliseconds(1));
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
            return new LogEvent(DateTimeOffset.MinValue,
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
            return ConnectionMultiplexer.Connect("127.0.0.1:6379");
        }

        public BasicRedisListSinkTests()
        {
            var db = GetDb();
            db.KeyDelete(key);
        }
    }
}
