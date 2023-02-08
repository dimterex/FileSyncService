namespace PublicProject.Logic
{
    using System.Collections.Generic;

    using _Interfaces_;
    using _Interfaces_.Factories;

    using Database.Actions.States;
    using Database.Actions.Users;

    using FileSystemProject;

    using Modules;

    using SdkProject.Api.Sync;

    public class SyncStateFilesResponseTask
    {
        private readonly IApiController _apiController;
        private readonly AvailableFoldersForUserRequestExecutor _availableFoldersForUserRequestExecutor;
        private readonly SyncStateFilesBodyRequest _bodyRequest;
        private readonly IFileInfoModelFactory _fileInfoModelFactory;
        private readonly IFileManager _fileManager;
        private readonly HttpRequestEventModel _httpRequestEventModel;
        private readonly string _login;
        private readonly ISyncStateFilesResponseFactory _syncStateFilesResponseFactory;
        private readonly ISyncStateFilesResponseService _syncStateFilesResponseService;
        private readonly SyncStatesRequestExecutor _syncStatesRequestExecutor;
        private readonly string _token;

        public SyncStateFilesResponseTask(
            string login,
            string token,
            SyncStateFilesBodyRequest bodyRequest,
            HttpRequestEventModel httpRequestEventModel,
            IFileManager fileManager,
            ISyncStateFilesResponseService syncStateFilesResponseService,
            IApiController apiController,
            ISyncStateFilesResponseFactory syncStateFilesResponseFactory,
            IFileInfoModelFactory fileInfoModelFactory,
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
            _fileInfoModelFactory = fileInfoModelFactory;
            _availableFoldersForUserRequestExecutor = availableFoldersForUserRequestExecutor;
            _syncStatesRequestExecutor = syncStatesRequestExecutor;
        }

        public void Run()
        {
            IList<string> availableFolders = _availableFoldersForUserRequestExecutor.Handler(_login);
            IList<string> syncStates = _syncStatesRequestExecutor.Handler(_login);

            var serverFiles = new List<DictionaryModel>();
            foreach (string folder in availableFolders)
            {
                var dictModel = new DictionaryModel(folder);
                serverFiles.Add(dictModel);
                IList<FileInfoModel> rootFiles = _fileManager.GetFiles(folder);

                foreach (FileInfoModel fileInfoModel in rootFiles)
                {
                    dictModel.Files.Add(fileInfoModel);
                }
            }

            SyncStateFilesResponse response = _syncStateFilesResponseFactory.Build(syncStates, _bodyRequest.Folders, serverFiles);

            _syncStateFilesResponseService.Remove(_login, _token);
            _syncStateFilesResponseService.Add(_login, _token, response);
            _apiController.SendResponse(_httpRequestEventModel, response);
        }
    }
}
