namespace PlaceholderService.Logic
{
    using System;

    using _Interfaces_;

    using Core.WebServiceBase._Interfaces_;
    using Core.WebServiceBase.Models;

    using Helper;

    using SdkProject.Api.Connection;

    public class ConnectionRequestTask
    {
        private readonly IApiController _apiController;
        private readonly IConnectionStateManager _connectionStateManager;
        private readonly HttpRequestEventModel _httpRequestEventModel;
        private readonly string _login;
        private readonly Guid _token;

        public ConnectionRequestTask(
            string login,
            HttpRequestEventModel httpRequestEventModel,
            IConnectionStateManager connectionStateManager,
            IApiController apiController)
        {
            _login = login;
            _httpRequestEventModel = httpRequestEventModel;
            _connectionStateManager = connectionStateManager;
            _apiController = apiController;
            _token = Guid.NewGuid();
        }

        public void Run()
        {
            _connectionStateManager.Add(_login, _token.ToString());

            var response = new ConnectionResponse
            {
                Token = _token.ToString()
            };
            
            var sharedFolder = new SharedFolder();
            sharedFolder.Files.AddRange(PathHelper.GetListOfPath("C:\\Users\\UseR\\Downloads\\test"));
            response.Shared_folders.Add(sharedFolder);

            _apiController.SendResponse(_httpRequestEventModel, response);
        }
    }
}
