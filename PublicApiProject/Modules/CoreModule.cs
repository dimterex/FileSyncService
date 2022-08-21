using System;
using System.Linq;
using Core.Logger;
using Core.Logger._Enums_;
using Core.Logger._Interfaces_;
using Core.Publisher;
using Core.Publisher._Interfaces_;
using FileSystemProject;
using PublicProject._Interfaces_;
using PublicProject._Interfaces_.Factories;
using PublicProject.Database.Actions.States;
using PublicProject.Helper;
using SdkProject.Api.Connection;
using SdkProject.Api.Sync;
using ServicesApi.Telegram;

namespace PublicProject.Modules
{
    public class CoreModule : BaseApiModule
    {
        private const string TAG = nameof(CoreModule);
        private readonly AddNewStatesExecutor _addNewStatesExecutor;
        private readonly IConnectionRequestTaskFactory _connectionRequestTaskFactory;
        private readonly IConnectionStateManager _connectionStateManager;

        private readonly IFileManager _fileManager;
        private readonly ILoggerService _loggerService;
        private readonly IPublisherService _publisherController;
        private readonly RemoveSyncStatesExecutor _removeSyncStatesExecutor;
        private readonly ISyncStateFilesResponseService _syncStateFilesResponseService;
        private readonly ISyncStateFilesResponseTaskFactory _syncStateFilesResponseTaskFactory;

        public CoreModule(IFileManager fileManager,
            IConnectionStateManager connectionStateManager,
            IRootService rootService,
            ISyncStateFilesResponseTaskFactory syncStateFilesResponseTaskFactory,
            ISyncStateFilesResponseService syncStateFilesResponseService,
            IConnectionRequestTaskFactory connectionRequestTaskFactory,
            AddNewStatesExecutor addNewStatesExecutor,
            RemoveSyncStatesExecutor removeSyncStatesExecutor,
            ApiController apiController,
            ILoggerService loggerService) : base("core", new Version(0, 1), apiController)
        {
            _fileManager = fileManager;
            _connectionStateManager = connectionStateManager;
            _publisherController = rootService.PublisherService;
            _syncStateFilesResponseTaskFactory = syncStateFilesResponseTaskFactory;
            _syncStateFilesResponseService = syncStateFilesResponseService;
            _connectionRequestTaskFactory = connectionRequestTaskFactory;
            _addNewStatesExecutor = addNewStatesExecutor;
            _removeSyncStatesExecutor = removeSyncStatesExecutor;
            _loggerService = loggerService;
        }

        protected override void OnInitialize()
        {
            RegisterPostRequestWithBody<SyncStateFilesBodyRequest>(OnSyncFilesRequest);
            RegisterPostRequestWithBody<SyncStartFilesRequest>(OnSyncStartFilesRequest);
            RegisterPostRequestWithBody<ConnectionRequest>(OnConnectionRequest);
        }

        private void OnSyncStartFilesRequest(SyncFilesRequest fileAction, SyncStartFilesRequest bodyRequest,
            HttpRequestEventModel e)
        {
            var login = _connectionStateManager.GetLoginByToken(fileAction.Token);
            if (string.IsNullOrEmpty(login))
            {
                _apiController.SetErrorResponse(e);
                _loggerService.SendLog(LogLevel.Error, TAG, () => $"Could find login for token: {fileAction.Token}");
                return;
            }

            var stateFilesResponse = _syncStateFilesResponseService.GetResponse(fileAction.Token);
            if (stateFilesResponse == null)
            {
                _apiController.SetErrorResponse(e);
                _loggerService.SendLog(LogLevel.Warning, TAG, () => $"Couldn't find sync state for {login}");
                return;
            }

            if (stateFilesResponse.DatabaseAddedFiles.Count > 0)
                _addNewStatesExecutor.Handler(login,
                    stateFilesResponse.DatabaseAddedFiles.Select(x => PathHelper.GetRawPath(x.FileName)).ToArray());

            if (stateFilesResponse.ServerRemovedFiles.Count > 0)
            {
                _removeSyncStatesExecutor.Handler(login,
                    stateFilesResponse.ServerRemovedFiles.Select(x => PathHelper.GetRawPath(x.FileName)).ToArray());

                foreach (var filePath in stateFilesResponse.ServerRemovedFiles.ToList())
                {
                    var path = PathHelper.GetRawPath(filePath.FileName);
                    _fileManager.RemoveFile(path);
                    _publisherController.SendMessage(new TelegramMessage
                    {
                        Message = $"Remove {path}"
                    });
                }
            }

            _syncStateFilesResponseService.Remove(login, fileAction.Token);
            _apiController.SendResponse(e, new SyncStartFilesResponse());
        }

        private void OnConnectionRequest(SyncFilesRequest arg2, ConnectionRequest connectionRequest,
            HttpRequestEventModel e)
        {
            var connectionRequestTask = _connectionRequestTaskFactory.Create(connectionRequest.Login, e);
            connectionRequestTask.Run();
        }

        private void OnSyncFilesRequest(SyncFilesRequest fileAction, SyncStateFilesBodyRequest bodyRequest,
            HttpRequestEventModel e)
        {
            var login = _connectionStateManager.GetLoginByToken(fileAction.Token);
            if (string.IsNullOrEmpty(login))
            {
                _apiController.SetErrorResponse(e);
                _loggerService.SendLog(LogLevel.Error, TAG, () => $"Could find login for token: {fileAction.Token}");
                return;
            }

            var connectionTask = _syncStateFilesResponseTaskFactory.Create(login, fileAction.Token, bodyRequest, e);
            connectionTask.Run();
        }
    }
}