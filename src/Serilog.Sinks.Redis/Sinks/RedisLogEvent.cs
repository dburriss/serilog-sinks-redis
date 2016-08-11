using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serilog.Events;

namespace Serilog.Sinks.Redis.Sinks
{
    public class RedisLogEvent
    {
        public RedisLogEvent()
        {
        }

        public RedisLogEvent(Events.LogEvent logEvent, string renderedMessage)
        {
            Timestamp = logEvent.Timestamp;
            Exception = logEvent.Exception;
            Message = logEvent.MessageTemplate.Text;
            Level = logEvent.Level;
            RenderedMessage = renderedMessage;
            Properties = new Dictionary<string, object>();
            foreach (var pair in logEvent.Properties)
            {
                Properties.Add(pair.Key, RedisSpecialScalarsPropertyFormatter.Simplify(pair.Value));
            }
        }

        //[JsonProperty("@timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        public string Message { get; set; }

        public LogEventLevel Level { get; set; }

        public Exception Exception { get; set; }

        public string RenderedMessage { get; set; }

        public IDictionary<string, object> Properties { get; set; }
    }
}