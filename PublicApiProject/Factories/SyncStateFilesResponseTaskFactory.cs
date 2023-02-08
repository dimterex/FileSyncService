namespace PublicProject.Factories
{
    using _Interfaces_;
    using _Interfaces_.Factories;

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
        private readonly IFileInfoModelFactory _fileInfoModelFactory;
        private readonly IFileManager _fileManager;
        private readonly ISyncStateFilesResponseFactory _syncStateFilesResponseFactory;
        private readonly ISyncStateFilesResponseService _syncStateFilesResponseService;
        private readonly SyncStatesRequestExecutor _syncStatesRequestExecutor;

        public SyncStateFilesResponseTaskFactory(
            IFileManager fileManager,
            IFileInfoModelFactory fileInfoModelFactory,
            ISyncStateFilesResponseService syncStateFilesResponseService,
            IApiController apiController,
            ISyncStateFilesResponseFactory syncStateFilesResponseFactory,
            AvailableFoldersForUserRequestExecutor availableFoldersForUserRequestExecutor,
            SyncStatesRequestExecutor syncStatesRequestExecutor)
        {
            _fileManager = fileManager;
            _fileInfoModelFactory = fileInfoModelFactory;
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
                _fileInfoModelFactory,
                _availableFoldersForUserRequestExecutor,
                _syncStatesRequestExecutor);
        }
    }
}
