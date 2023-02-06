using FileSystemProject;
using PublicProject._Interfaces_;
using PublicProject._Interfaces_.Factories;
using PublicProject.Database.Actions.States;
using PublicProject.Database.Actions.Users;
using PublicProject.Logic;
using PublicProject.Modules;
using SdkProject.Api.Sync;

namespace PublicProject.Factories
{
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

        public SyncStateFilesResponseTask Create(string login, string token, SyncStateFilesBodyRequest bodyRequest,
            HttpRequestEventModel e)
        {
            return new SyncStateFilesResponseTask(login, token, bodyRequest, e,
                _fileManager, _syncStateFilesResponseService,
                _apiController,
                _syncStateFilesResponseFactory,
                _availableFoldersForUserRequestExecutor,
                _syncStatesRequestExecutor);
        }
    }
}