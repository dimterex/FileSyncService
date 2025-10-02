namespace PublicProject.Modules
{
    using System;

    using _Interfaces_;
    using _Interfaces_.Factories;

    using Core.WebServiceBase._Interfaces_;
    using Core.WebServiceBase.Models;

    using Logic;

    using NLog;

    using SdkProject.Api.Connection;
    using SdkProject.Api.Sync;

    public class CoreModule : BaseApiModule
    {
        private const string TAG = nameof(CoreModule);
        private readonly IConnectionRequestTaskFactory _connectionRequestTaskFactory;
      
        private readonly ILogger _logger;

        public CoreModule(
            IConnectionRequestTaskFactory connectionRequestTaskFactory,
            IApiController apiController)
            : base("core", apiController)
        {
            _connectionRequestTaskFactory = connectionRequestTaskFactory;
            _logger = LogManager.GetLogger(TAG);
        }

        protected override void OnInitialize()
        {
            RegisterPostRequestWithBody<ConnectionRequest>(OnConnectionRequest);
        }

        private void OnConnectionRequest(SyncFilesRequest arg2, ConnectionRequest connectionRequest, HttpRequestEventModel e)
        {
            ConnectionRequestTask connectionRequestTask = _connectionRequestTaskFactory.Create(connectionRequest.Login, e);
            connectionRequestTask.Run();
        }
    }
}
