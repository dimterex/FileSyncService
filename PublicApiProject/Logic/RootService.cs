using System;
using Core.Customer;
using Core.Logger;
using Core.Logger._Enums_;
using Core.Logger._Interfaces_;
using Core.Publisher;
using Core.Publisher._Interfaces_;
using FileSystemProject;
using PublicApiProject;
using PublicProject._Interfaces_;
using PublicProject.Actions;
using ServicesApi;
using ServicesApi.Logger;

namespace PublicProject.Logic
{
    public class RootService : IRootService
    {
        private readonly CustomerController _customerController;
        private readonly WsService _wsService;

        public RootService(WsService wsService, IFileManager fileManager, ILoggerService loggerService)
        {
            var host = Environment.GetEnvironmentVariable(Program.RABBIT_HOST);

            _wsService = wsService;

            PublisherService = new PublisherService(host, loggerService);
            loggerService.LogMessageEvent += LoggerServiceOnLogMessageEvent;
            _customerController = new CustomerController(host, QueueConstants.FILE_STORAGE_QUEUE, loggerService);

            _customerController.Configure(new ClearEmptyDirectoriesAction(PublisherService, fileManager));
        }

        public IPublisherService PublisherService { get; }

        public void Start(int httpPort, int httpsPort)
        {
            _wsService.Start(httpPort, httpsPort);
        }

        private void LoggerServiceOnLogMessageEvent(object sender, LogMessage e)
        {
            var loglevel = string.Empty;
            switch (e.Level)
            {
                case LogLevel.Trace:
                    loglevel = "TRACE";
                    break;
                case LogLevel.Debug:
                    loglevel = "DEBUG";
                    break;
                case LogLevel.Info:
                    loglevel = "INFO";
                    break;
                case LogLevel.Warning:
                    loglevel = "WARNING";
                    break;
                case LogLevel.Error:
                    loglevel = "ERROR";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PublisherService.SendMessage(new LoggerMessage
            {
                Level = loglevel,
                Datetime = e.Datetime.ToString(),
                Application = Program.TAG,
                Message = e.Message(),
                Tag = e.Tag
            });
        }
    }
}