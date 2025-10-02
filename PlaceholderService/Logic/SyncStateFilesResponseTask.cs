namespace PlaceholderService.Logic
{
    using _Interfaces_;

    using Core.WebServiceBase._Interfaces_;
    using Core.WebServiceBase.Models;

    using FileSystemProject;

    using Helper;

    using Modules;

    using NLog;

    using SdkProject.Api.Sync;

    public class SyncStateFilesResponseTask
    {
        private readonly IApiController _apiController;
        private readonly HttpRequestEventModel _httpRequestEventModel;
        private readonly string _login;
        private readonly ISyncStateFilesResponseService _syncStateFilesResponseService;
        private readonly string _token;
        private readonly ILogger _logger;

        public SyncStateFilesResponseTask(
            string login,
            string token,
            HttpRequestEventModel httpRequestEventModel,
            ISyncStateFilesResponseService syncStateFilesResponseService,
            IApiController apiController)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _login = login;
            _token = token;
            _httpRequestEventModel = httpRequestEventModel;
            _syncStateFilesResponseService = syncStateFilesResponseService;
            _apiController = apiController;
        }

        public void Run()
        {
            SyncStateFilesResponse response = new SyncStateFilesResponse();

            var count = Random.Shared.Next(0, 100);
            _logger.Trace(() => $"Added count: {count}");
            for (int i = 0; i < count; i++)
                response.AddedFiles.Add(Constants.FILE_ADD_RESPONSE);
            

            _syncStateFilesResponseService.Remove(_login, _token);
            _syncStateFilesResponseService.Add(_login, _token, response);
            _apiController.SendResponse(_httpRequestEventModel, response);
        }
    }
}
