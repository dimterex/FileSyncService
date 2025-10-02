namespace PlaceholderService.Modules
{
    using System;
    using System.Linq;

    using _Interfaces_;

    using Core.WebServiceBase._Interfaces_;
    using Core.WebServiceBase.Models;

    using FileSystemProject;

    using Helper;

    using Logic;

    using NLog;

    using SdkProject.Api.Connection;
    using SdkProject.Api.Sync;

    public class CoreModule : BaseApiModule
    {
        private const string TAG = nameof(CoreModule);
        private readonly IConnectionStateManager _connectionStateManager;

        private readonly IFileManager _fileManager;
        private readonly ILogger _logger;
        private readonly ISyncStateFilesResponseService _syncStateFilesResponseService;

        public CoreModule(
            IFileManager fileManager,
            IConnectionStateManager connectionStateManager,
            ISyncStateFilesResponseService syncStateFilesResponseService,
            IApiController apiController)
            : base("core", apiController)
        {
            _fileManager = fileManager;
            _connectionStateManager = connectionStateManager;
            _syncStateFilesResponseService = syncStateFilesResponseService;
            _logger = LogManager.GetLogger(TAG);
        }

        protected override void OnInitialize()
        {
            RegisterPostRequestWithBody<SyncStateFilesBodyRequest>(OnSyncFilesRequest);
            RegisterPostRequestWithBody<SyncStartFilesRequest>(OnSyncStartFilesRequest);
            RegisterPostRequestWithBody<ConnectionRequest>(OnConnectionRequest);
        }

        private void OnSyncStartFilesRequest(SyncFilesRequest fileAction, SyncStartFilesRequest bodyRequest, HttpRequestEventModel e)
        {
            string login = _connectionStateManager.GetLoginByToken(fileAction.Token);
            if (string.IsNullOrEmpty(login))
            {
                _apiController.SetErrorResponse(e);
                _logger.Error(() => $"Could find login for token: {fileAction.Token}");
                return;
            }

            SyncStateFilesResponse stateFilesResponse = _syncStateFilesResponseService.GetResponse(fileAction.Token);
            if (stateFilesResponse == null)
            {
                _apiController.SetErrorResponse(e);
                _logger.Error(() => $"Couldn't find sync state for {login}");
                return;
            }

            if (stateFilesResponse.ServerRemovedFiles.Count > 0)
            {
                foreach (FileServerRemovedResponse filePath in stateFilesResponse.ServerRemovedFiles.ToList())
                {
                    string path = PathHelper.GetRawPath(filePath.FileName);
                    _fileManager.RemoveFile(path);
                }
            }

            _syncStateFilesResponseService.Remove(login, fileAction.Token);
            _apiController.SendResponse(e, new SyncStartFilesResponse());
        }

        private void OnConnectionRequest(SyncFilesRequest arg2, ConnectionRequest connectionRequest, HttpRequestEventModel e)
        {
            ConnectionRequestTask connectionRequestTask = new ConnectionRequestTask(connectionRequest.Login, e, _connectionStateManager, _apiController);
            connectionRequestTask.Run();
        }

        private void OnSyncFilesRequest(SyncFilesRequest fileAction, SyncStateFilesBodyRequest bodyRequest, HttpRequestEventModel e)
        {
            SyncStateFilesResponseTask connectionTask = new SyncStateFilesResponseTask(fileAction.Token, fileAction.Token, e, _syncStateFilesResponseService, _apiController);
            connectionTask.Run();
        }
    }
}
