namespace PublicProject.Factories
{
    using _Interfaces_;
    using _Interfaces_.Factories;

    using Core.WebServiceBase._Interfaces_;
    using Core.WebServiceBase.Models;

    using Database.Actions.States;
    using Database.Actions.Users;

    using FileSystemProject;

    using Logic;

    using Modules;

    using SdkProject.Api.Sync;

    public class SyncStateFilesResponseTaskFactory : ISyncStateFilesResponseTaskFactory
    {
        private readonly IApiController _apiController;
        private readonly AvailableFoldersForUserRequestExecutor _availableFoldersForUserRequestExecutor;
        private readonly IFileManager _fileManager;
        private readonly ISyncStateFilesResponseFactory _syncStateFilesResponseFactory;
        private readonly ISyncStateFilesResponseService _syncStateFilesResponseService;
        private readonly SyncStatesRequestExecutor _syncStatesRequestExecutor;

        public SyncStateFilesResponseTaskFactory(
            IFileManager fileManager,
            ISyncStateFilesResponseService syncStateFilesResponseService,
            IApiController apiController,
            ISyncStateFilesResponseFactory syncStateFilesResponseFactory,
            AvailableFoldersForUserRequestExecutor availableFoldersForUserRequestExecutor,
            SyncStatesRequestExecutor syncStatesRequestExecutor)
        {
            _fileManager = fileManager;
            _syncStateFilesResponseService = syncStateFilesResponseService;
            _apiController = apiController;
            _syncStateFilesResponseFactory = syncStateFilesResponseFactory;
            _availableFoldersForUserRequestExecutor = availableFoldersForUserRequestExecutor;
            _syncStatesRequestExecutor = syncStatesRequestExecutor;
        }

        public SyncStateFilesResponseTask Create(string login, string token, SyncStateFilesBodyRequest bodyRequest, HttpRequestEventModel e)
        {
            return new SyncStateFilesResponseTask(
                login,
                token,
                bodyRequest,
                e,
                _fileManager,
                _syncStateFilesResponseService,
                _apiController,
                _syncStateFilesResponseFactory,
                _availableFoldersForUserRequestExecutor,
                _syncStatesRequestExecutor);
        }
    }
}
