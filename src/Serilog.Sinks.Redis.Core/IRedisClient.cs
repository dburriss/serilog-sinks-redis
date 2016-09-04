using System;
using System.Collections.Generic;
using Serilog.Events;

namespace Serilog.Sinks.Redis.Core
{
    public interface IRedisClient : IDisposable
    {
        void Send(IEnumerable<LogEvent> events);
    }
}
