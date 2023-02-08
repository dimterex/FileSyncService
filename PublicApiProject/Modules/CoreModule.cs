namespace PublicProject.Modules
{
    using System;
    using System.Linq;

    using _Interfaces_;
    using _Interfaces_.Factories;

    using Core.Publisher._Interfaces_;

    using Database.Actions.States;

    using FileSystemProject;

    using Helper;

    using Logic;

    using NLog;

    using SdkProject.Api.Connection;
    using SdkProject.Api.Sync;

    public class CoreModule : BaseApiModule
    {
        private const string TAG = nameof(CoreModule);
        private readonly AddNewStatesExecutor _addNewStatesExecutor;
        private readonly IConnectionRequestTaskFactory _connectionRequestTaskFactory;
        private readonly IConnectionStateManager _connectionStateManager;

        private readonly IFileManager _fileManager;
        private readonly IHistoryService _historyService;
        private readonly ILogger _logger;
        private readonly IPublisherService _publisherController;
        private readonly RemoveSyncStatesExecutor _removeSyncStatesExecutor;
        private readonly ISyncStateFilesResponseService _syncStateFilesResponseService;
        private readonly ISyncStateFilesResponseTaskFactory _syncStateFilesResponseTaskFactory;

        public CoreModule(
            IFileManager fileManager,
            IConnectionStateManager connectionStateManager,
            IRootService rootService,
            ISyncStateFilesResponseTaskFactory syncStateFilesResponseTaskFactory,
            ISyncStateFilesResponseService syncStateFilesResponseService,
            IConnectionRequestTaskFactory connectionRequestTaskFactory,
            AddNewStatesExecutor addNewStatesExecutor,
            RemoveSyncStatesExecutor removeSyncStatesExecutor,
            IApiController apiController,
            IHistoryService historyService)
            : base("core", new Version(0, 1), apiController)
        {
            _fileManager = fileManager;
            _connectionStateManager = connectionStateManager;
            _publisherController = rootService.PublisherService;
            _syncStateFilesResponseTaskFactory = syncStateFilesResponseTaskFactory;
            _syncStateFilesResponseService = syncStateFilesResponseService;
            _connectionRequestTaskFactory = connectionRequestTaskFactory;
            _addNewStatesExecutor = addNewStatesExecutor;
            _removeSyncStatesExecutor = removeSyncStatesExecutor;
            _historyService = historyService;
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

            if (stateFilesResponse.DatabaseAddedFiles.Count > 0)
                _addNewStatesExecutor.Handler(login, stateFilesResponse.DatabaseAddedFiles.Select(x => PathHelper.GetRawPath(x.FileName)).ToArray());

            if (stateFilesResponse.ServerRemovedFiles.Count > 0)
            {
                _removeSyncStatesExecutor.Handler(
                    login,
                    stateFilesResponse.ServerRemovedFiles.Select(x => PathHelper.GetRawPath(x.FileName)).ToArray());

                foreach (FileServerRemovedResponse filePath in stateFilesResponse.ServerRemovedFiles.ToList())
                {
                    string path = PathHelper.GetRawPath(filePath.FileName);
                    _fileManager.RemoveFile(path);
                    _historyService.AddNewEvent(login, path, "Removed");
                }
            }

            _syncStateFilesResponseService.Remove(login, fileAction.Token);
            _apiController.SendResponse(e, new SyncStartFilesResponse());
        }

        private void OnConnectionRequest(SyncFilesRequest arg2, ConnectionRequest connectionRequest, HttpRequestEventModel e)
        {
            ConnectionRequestTask connectionRequestTask = _connectionRequestTaskFactory.Create(connectionRequest.Login, e);
            connectionRequestTask.Run();
        }

        private void OnSyncFilesRequest(SyncFilesRequest fileAction, SyncStateFilesBodyRequest bodyRequest, HttpRequestEventModel e)
        {
            string login = _connectionStateManager.GetLoginByToken(fileAction.Token);
            if (string.IsNullOrEmpty(login))
            {
                _apiController.SetErrorResponse(e);
                _logger.Error(() => $"Could find login for token: {fileAction.Token}");
                return;
            }

            SyncStateFilesResponseTask connectionTask = _syncStateFilesResponseTaskFactory.Create(login, fileAction.Token, bodyRequest, e);
            connectionTask.Run();
        }
    }
}
