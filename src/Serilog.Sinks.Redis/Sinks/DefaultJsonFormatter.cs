using System;
using System.Collections.Generic;
using Serilog.Events;

namespace Serilog.Sinks.Redis.Sinks
{
    public class DefaultJsonFormatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;

            return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var type = arg.GetType();

            if (type == typeof (IEnumerable<LogEvent>))
            {   
                return string.Format(format, arg);
            }

            return arg.ToString();
        }
    }
}