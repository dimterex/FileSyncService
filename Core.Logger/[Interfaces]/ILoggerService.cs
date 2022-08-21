using System;
using Core.Logger._Enums_;

namespace Core.Logger._Interfaces_
{
    public interface ILoggerService
    {
        event EventHandler<LogMessage> LogMessageEvent;
        void SendLog(LogLevel level, string tag, Func<string> message);
    }
}