using System;
using Core.Customer;
using Core.Logger;
using Core.Logger._Enums_;
using Core.Logger._Interfaces_;
using Core.Publisher;
using ServicesApi.Logger;

namespace TelegramBotService
{
    public class RootService
    {
        private readonly CustomerController _customerController;
        private readonly PublisherService _publisherService;

        public RootService(ILoggerService loggerService, PublisherService publisherService)
        {
            _publisherService = publisherService;
            loggerService.LogMessageEvent += LoggerServiceOnLogMessageEvent;
        }

        private void LoggerServiceOnLogMessageEvent(object sender, LogMessage logMessage)
        {
            var loglevel = logMessage.Level switch
            {
                LogLevel.Trace => "TRACE",
                LogLevel.Debug => "DEBUG",
                LogLevel.Info => "INFO",
                LogLevel.Warning => "WARNING",
                LogLevel.Error => "ERROR",
                _ => throw new ArgumentOutOfRangeException()
            };

            _publisherService.SendMessage(new LoggerMessage
            {
                Level = loglevel,
                Datetime = logMessage.Datetime.ToString(),
                Application = Program.TAG,
                Message = logMessage.Message(),
                Tag = logMessage.Tag
            });
        }
    }
}