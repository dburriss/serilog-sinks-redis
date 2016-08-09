using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FakeItEasy;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Redis.Sinks;
using StackExchange.Redis;
using Xunit;

namespace Serilog.Sinks.Redis.Tests
{
    public class BasicRedisListSinkTests
    {
        private const string key = "testKey";

        [Fact(Skip = "fake?")]
        public void Emit_KnownEvent_SendsEventToDatabase()
        {
            var connection = A.Fake<IConnectionMultiplexer>();
            var db = A.Fake<IDatabase>();
            A.CallTo(() => connection.GetDatabase(A<int>.Ignored, A<object>.Ignored)).Returns(db);
            var sut = new BasicRedisListSink(connection, key, TimeSpan.FromMilliseconds(1));

            sut.Emit(CreateLogItem());

            A.CallTo(() => db.ListLeftPush(A<string>.That.Matches(x => x == key), A<RedisValue>.Ignored, A<When>.Ignored, A<CommandFlags>.Ignored))
                .MustHaveHappened(Repeated.AtLeast.Once);
        }

        [Fact(Skip = "fake?")]
        public void Emit_KnownEvent_SendsJsonToDatabase()
        {
            var connection = A.Fake<IConnectionMultiplexer>();
            var db = A.Fake<IDatabase>();
            string result = "";
            A.CallTo(() => connection.GetDatabase(A<int>.Ignored, A<object>.Ignored)).Returns(db);
            A.CallTo(() => db.ListLeftPush(key, A<RedisValue>.Ignored, A<When>.Ignored, A<CommandFlags>.Ignored))
                .Invokes(f => result = f.GetArgument<string>(1));

            var sut = new BasicRedisListSink(connection, key, TimeSpan.FromMilliseconds(1));
            sut.Emit(CreateLogItem());

            Assert.NotEmpty(result);
        }

        private static LogEvent CreateLogItem()
        {
            return new LogEvent(DateTimeOffset.MinValue,
                LogEventLevel.Information,
                new Exception("TEST"),
                new MessageTemplate(
                    "This is a test on {Date}", 
                    new List<MessageTemplateToken>() { new TextToken("Date") }
                    ),
                new List<LogEventProperty>());
        }
    }
}
