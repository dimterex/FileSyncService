using System.Collections.Generic;
using FileSystemProject;
using PublicProject._Interfaces_;
using PublicProject._Interfaces_.Factories;
using PublicProject.Database.Actions.States;
using PublicProject.Database.Actions.Users;
using PublicProject.Modules;
using SdkProject.Api.Sync;

namespace PublicProject.Logic
{
    public class SyncStateFilesResponseTask
    {
        private readonly IApiController _apiController;
        private readonly AvailableFoldersForUserRequestExecutor _availableFoldersForUserRequestExecutor;
        private readonly SyncStateFilesBodyRequest _bodyRequest;
        private readonly IFileManager _fileManager;
        private readonly HttpRequestEventModel _httpRequestEventModel;
        private readonly string _login;
        private readonly ISyncStateFilesResponseFactory _syncStateFilesResponseFactory;
        private readonly ISyncStateFilesResponseService _syncStateFilesResponseService;
        private readonly SyncStatesRequestExecutor _syncStatesRequestExecutor;
        private readonly string _token;

        public SyncStateFilesResponseTask(string login,
            string token,
            SyncStateFilesBodyRequest bodyRequest,
            HttpRequestEventModel httpRequestEventModel,
            IFileManager fileManager,
            ISyncStateFilesResponseService syncStateFilesResponseService,
            IApiController apiController,
            ISyncStateFilesResponseFactory syncStateFilesResponseFactory,
            AvailableFoldersForUserRequestExecutor availableFoldersForUserRequestExecutor,
            SyncStatesRequestExecutor syncStatesRequestExecutor)
        {
            _login = login;
            _token = token;
            _bodyRequest = bodyRequest;
            _httpRequestEventModel = httpRequestEventModel;
            _fileManager = fileManager;
            _syncStateFilesResponseService = syncStateFilesResponseService;
            _apiController = apiController;
            _syncStateFilesResponseFactory = syncStateFilesResponseFactory;
            _availableFoldersForUserRequestExecutor = availableFoldersForUserRequestExecutor;
            _syncStatesRequestExecutor = syncStatesRequestExecutor;
        }

        public void Run()
        {
            var availableFolders = _availableFoldersForUserRequestExecutor.Handler(_login);
            var syncStates = _syncStatesRequestExecutor.Handler(_login);

            var serverFiles = new List<DictionaryModel>();
            foreach (var folder in availableFolders)
            {
                var dictModel = new DictionaryModel(folder);
                serverFiles.Add(dictModel);
                var rootFiles = _fileManager.GetFiles(folder);

                foreach (var fileInfoModel in rootFiles)
                    dictModel.Files.Add(new FileInfoModel(fileInfoModel.Path, fileInfoModel.Size));
            }

            var response = _syncStateFilesResponseFactory.Build(syncStates, _bodyRequest.Folders, serverFiles);

            _syncStateFilesResponseService.Remove(_login, _token);
            _syncStateFilesResponseService.Add(_login, _token, response);
            _apiController.SendResponse(_httpRequestEventModel, response);
        }
    }
}