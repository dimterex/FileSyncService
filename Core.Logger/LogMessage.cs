using System;
using Core.Logger._Enums_;

namespace Core.Logger
{
    public class LogMessage
    {
        public LogMessage(LogLevel level, string tag, Func<string> message)
        {
            Level = level;
            Tag = tag;
            Message = message;
            Datetime = DateTime.Now;
        }

        public LogLevel Level { get; }
        public string Tag { get; }
        public DateTime Datetime { get; }
        public Func<string> Message { get; }
    }
}