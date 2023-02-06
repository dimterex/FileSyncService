using System;
using Core.Customer;
using Core.Publisher;
using Core.Publisher._Interfaces_;
using FileSystemProject;
using PublicApiProject;
using PublicProject._Interfaces_;
using PublicProject.Actions;
using ServicesApi;

namespace PublicProject.Logic
{
    public class RootService : IRootService
    {
        private readonly CustomerController _customerController;
        private readonly WsService _wsService;

        public RootService(WsService wsService, IFileManager fileManager)
        {
            var host = Environment.GetEnvironmentVariable(Program.RABBIT_HOST);

            _wsService = wsService;

            PublisherService = new PublisherService(host);
            _customerController = new CustomerController(host, QueueConstants.FILE_STORAGE_QUEUE);

            _customerController.Configure(new ClearEmptyDirectoriesAction(PublisherService, fileManager));
        }

        public IPublisherService PublisherService { get; }

        public void Start(int httpPort, int httpsPort)
        {
            _wsService.Start(httpPort, httpsPort);
        }
    }
}