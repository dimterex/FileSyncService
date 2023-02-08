namespace PublicProject.Logic
{
    using System;

    using _Interfaces_;

    using Actions;

    using Core.Customer;
    using Core.Publisher;
    using Core.Publisher._Interfaces_;

    using PublicApiProject;

    using ServicesApi;

    public class RootService : IRootService
    {
        private readonly CustomerController _customerController;
        private readonly WsService _wsService;

        public RootService(
            WsService wsService,
            ClearEmptyDirectoriesAction clearEmptyDirectoriesAction,
            GetHistoryAction getHistoryAction,
            CreateUserAction createUserAction,
            UpdateSyncStateAction updateSyncStateAction)
        {
            string host = Environment.GetEnvironmentVariable(Program.RABBIT_HOST);

            _wsService = wsService;

            PublisherService = new RpcPublisherService(host);
            _customerController = new CustomerController(host, QueueConstants.FILE_STORAGE_QUEUE);

            _customerController.Configure(clearEmptyDirectoriesAction);
            _customerController.Configure(getHistoryAction);
            _customerController.Configure(createUserAction);
            _customerController.Configure(updateSyncStateAction);
        }

        public IPublisherService PublisherService { get; }

        public void Start(int httpPort, int httpsPort)
        {
            _wsService.Start(httpPort, httpsPort);
        }
    }
}
