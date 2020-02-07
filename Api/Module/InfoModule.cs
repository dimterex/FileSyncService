namespace Service.Api.Module
{
    using NLog;

    using Service.Api.Message.Info;
    using Service.Transport;

    public class InfoModule
    {
        private readonly FileManager _fileManager;
        private readonly ApiController _apiController;
        private readonly ILogger _logger;

        public InfoModule(ApiController jsonManager, FileManager fileManager)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _fileManager = fileManager;
            _apiController = jsonManager;
            _apiController.Configure<FileListRequest>(OnFileListRequest);
        }

        private void OnFileListRequest(FileListRequest fileAddResponce, IClient client)
        {
            var result = new FileListResponce();
            var tmp = _fileManager.GetFileList();
            result.Files.AddRange(tmp);
            _apiController.Send(client, result);

            _logger.Info("FileListResponce Done!");
        }
    }
}
