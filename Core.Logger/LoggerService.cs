using System;
using Core.Logger._Enums_;
using Core.Logger._Interfaces_;

namespace Core.Logger
{
    public class LoggerService : ILoggerService
    {
        public event EventHandler<LogMessage> LogMessageEvent;

        public void SendLog(LogLevel level, string tag, Func<string> message)
        {
            var logMessage = new LogMessage(level, tag, message);

            LogMessageEvent?.Invoke(this, logMessage);
        }
    }
}