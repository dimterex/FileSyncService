namespace PublicProject.Logic
{
    using System;
    using System.Collections.Generic;

    using _Interfaces_;

    using Core.WebServiceBase._Interfaces_;
    using Core.WebServiceBase.Models;

    using Database.Actions.Users;

    using Helper;

    using Modules;

    using SdkProject.Api.Connection;

    public class ConnectionRequestTask
    {
        private readonly IApiController _apiController;
        private readonly AvailableFoldersForUserRequestExecutor _availableFoldersForUserRequestExecutor;
        private readonly IConnectionStateManager _connectionStateManager;
        private readonly HttpRequestEventModel _httpRequestEventModel;
        private readonly string _login;
        private readonly Guid _token;

        public ConnectionRequestTask(
            string login,
            HttpRequestEventModel httpRequestEventModel,
            IConnectionStateManager connectionStateManager,
            AvailableFoldersForUserRequestExecutor availableFoldersForUserRequestExecutor,
            IApiController apiController)
        {
            _login = login;
            _httpRequestEventModel = httpRequestEventModel;
            _connectionStateManager = connectionStateManager;
            _availableFoldersForUserRequestExecutor = availableFoldersForUserRequestExecutor;
            _apiController = apiController;
            _token = Guid.NewGuid();
        }

        public void Run()
        {
            _connectionStateManager.Add(_login, _token.ToString());

            IList<string> availableFolders = _availableFoldersForUserRequestExecutor.Handler(_login);

            var response = new ConnectionResponse
            {
                Token = _token.ToString()
            };

            foreach (string folder in availableFolders)
            {
                var sharedFolder = new SharedFolder();
                sharedFolder.Files.AddRange(PathHelper.GetListOfPath(folder));
                response.Shared_folders.Add(sharedFolder);
            }

            _apiController.SendResponse(_httpRequestEventModel, response);
        }
    }
}
