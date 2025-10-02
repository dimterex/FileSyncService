namespace PublicProject.Modules;

using System.Linq;

using _Interfaces_;
using _Interfaces_.Factories;

using Core.WebServiceBase._Interfaces_;
using Core.WebServiceBase.Models;

using Database.Actions.States;

using FileSystemProject;

using Helper;

using Logic;

using NLog;

using SdkProject.Api.Sync;

public class SyncStateModule : BaseApiModule
{
    private const string TAG = nameof(SyncStateModule);
    private readonly AddNewStatesExecutor _addNewStatesExecutor;
    private readonly IConnectionStateManager _connectionStateManager;

    private readonly IFileManager _fileManager;
    private readonly IHistoryService _historyService;
    private readonly ILogger _logger;
    private readonly RemoveSyncStatesExecutor _removeSyncStatesExecutor;
    private readonly ISyncStateFilesResponseService _syncStateFilesResponseService;
    private readonly ISyncStateFilesResponseTaskFactory _syncStateFilesResponseTaskFactory;
    
    public SyncStateModule(IFileManager fileManager,
                           IConnectionStateManager connectionStateManager,
                           ISyncStateFilesResponseTaskFactory syncStateFilesResponseTaskFactory,
                           ISyncStateFilesResponseService syncStateFilesResponseService,
                           AddNewStatesExecutor addNewStatesExecutor,
                           RemoveSyncStatesExecutor removeSyncStatesExecutor,
                           IApiController apiController,
                           IHistoryService historyService)
        : base("sync", apiController)
    {
        _fileManager = fileManager;
        _connectionStateManager = connectionStateManager;
        _syncStateFilesResponseTaskFactory = syncStateFilesResponseTaskFactory;
        _syncStateFilesResponseService = syncStateFilesResponseService;
        _addNewStatesExecutor = addNewStatesExecutor;
        _removeSyncStatesExecutor = removeSyncStatesExecutor;
        _historyService = historyService;
        _logger = LogManager.GetLogger(TAG);
    }

    protected override void OnInitialize()
    {
        RegisterPostRequestWithBody<SyncStateFilesBodyRequest>(OnSyncFilesRequest);
        RegisterPostRequestWithBody<SyncStartFilesRequest>(OnSyncStartFilesRequest);
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
}
