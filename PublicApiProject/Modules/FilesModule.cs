using System;
using System.IO;
using System.Net;
using System.Text;
using Core.Publisher._Interfaces_;
using Newtonsoft.Json;
using NLog;
using PublicProject._Interfaces_;
using PublicProject.Database.Actions.States;
using SdkProject.Api.Files;
using ServicesApi.Telegram;

namespace PublicProject.Modules
{
    public class FilesModule : BaseApiModule
    {
        #region Constructors

        public FilesModule(IConnectionStateManager connectionStateManager,
            IRootService rootService,
            AddNewStateExecutor addNewStateExecutor,
            IApiController apiController,
            IHistoryService historyService) : base("files", new Version(0, 1), apiController)
        {
            _connectionStateManager = connectionStateManager;
            _publisherController = rootService.PublisherService;
            _addNewStateExecutor = addNewStateExecutor;
            _historyService = historyService;
            _logger = LogManager.GetLogger(TAG);
        }

        #endregion

        #region Constants

        private const string UPLOAD_REQUEST_NAME = "upload";

        private const string DOWNLOAD_REQUEST_NAME = "download";

        private const int READ_FILE_BUFFER_SIZE = 81920;
        private const string TAG = nameof(FilesModule);

        #endregion

        #region Fields

        private readonly IConnectionStateManager _connectionStateManager;
        private readonly IPublisherService _publisherController;
        private readonly AddNewStateExecutor _addNewStateExecutor;
        private readonly IHistoryService _historyService;
        private readonly Logger _logger;

        #endregion

        #region Methods

        protected override void OnInitialize()
        {
            // Регистрация обработчиков для REST-запросов:
            RegisterPostRequest<UploadRequest>(UPLOAD_REQUEST_NAME, HandleUploadRequest);
            RegisterGetRequest<DownloadRequest>(DOWNLOAD_REQUEST_NAME, HandleDownloadRequest);
        }

        private void HandleUploadRequest(UploadRequest request, HttpRequestEventModel e)
        {
            _logger.Debug(() => $"Upload {request.FileName}");

            var login = _connectionStateManager.GetLoginByToken(request.Token);

            var fileCrutch = JsonConvert.DeserializeObject<FileCrutch>(request.FileName);

            if (fileCrutch == null)
            {
                e.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.Warn(() => "Empty filename");
                return;
            }

            var rawPath = GetRawPath(fileCrutch.FileName);

            var isValidRequest = HandleUploadRequest(
                login,
                rawPath,
                e.Request.InputStream,
                e.Request.ContentLength64,
                out var response,
                out var errorStatusCode,
                out var errorMessage);

            if (!isValidRequest)
            {
                e.Response.StatusCode = (int)errorStatusCode;
                _logger.Warn(() => errorMessage);
            }

            _publisherController.SendMessage(new TelegramMessage
            {
                Message = $"Added from {login}: {request.FileName}"
            });

            e.Response.SendChunked = true;
            using (var streamWriter = new StreamWriter(e.Response.OutputStream, Encoding.UTF8))
            {
                var serializer = JsonSerializer.Create(new JsonSerializerSettings
                    { NullValueHandling = NullValueHandling.Ignore });
                var jsonWriter = new JsonTextWriter(streamWriter);

                try
                {
                    serializer.Serialize(jsonWriter, response);
                }
                finally
                {
                    jsonWriter.Close();
                }
            }
        }

        private void HandleDownloadRequest(DownloadRequest request, HttpRequestEventModel e)
        {
            e.Response.SendChunked = true;

            _logger.Debug(() => $"Download {request.FilePath}");
            if (HandleDownloadRequest(request, e.Response.OutputStream, out var errorStatusCode, out var errorMessage))
                return;

            e.Response.StatusCode = (int)errorStatusCode;
            _logger.Debug(() => errorMessage);
        }

        private bool HandleUploadRequest(
            string login,
            string filePath,
            Stream stream,
            long contentLength,
            out UploadResponse response,
            out HttpStatusCode errorStatusCode,
            out string errorMessage)
        {
            response = new UploadResponse();
            errorStatusCode = HttpStatusCode.Forbidden;

            if (contentLength < 0)
            {
                response.Result = FilesOperationResult.UnexpectedError;
                errorMessage = "Unable to determine file size";
                return false;
            }

            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
                _logger.Info(() => $"Directory {fileInfo.Directory} created.");
            }

            using (var fileStream = File.Create(filePath))
            {
                stream.CopyTo(fileStream);
            }

            response.Result = FilesOperationResult.Success;
            // response.FileId = Guid.NewGuid().ToString();

            errorStatusCode = HttpStatusCode.OK;


            _addNewStateExecutor.Handler(login, filePath);
            _historyService.AddNewEvent(login, filePath, "Uploaded");

            errorMessage = null;

            return true;
        }

        private bool HandleDownloadRequest(DownloadRequest request, Stream stream, out HttpStatusCode errorStatusCode, out string errorMessage)
        {
            errorStatusCode = HttpStatusCode.Forbidden;

            var fileCrutch = JsonConvert.DeserializeObject<FileCrutch>(request.FilePath);

            if (fileCrutch == null)
            {
                errorStatusCode = HttpStatusCode.NotFound;
                errorMessage = $"File {request.FilePath} is not exist!";
                return false;
            }

            var rawPath = GetRawPath(fileCrutch.FileName);
            if (string.IsNullOrEmpty(rawPath) || !File.Exists(rawPath))
            {
                errorStatusCode = HttpStatusCode.NotFound;
                errorMessage = $"File {request.FilePath} is not exist!";
                return false;
            }

            using (Stream reader = File.OpenRead(rawPath))
            {
                var buffer = new byte[READ_FILE_BUFFER_SIZE];

                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, count);
                    stream.Flush();
                    _logger.Trace(() => $"Sending {request.FilePath} ${count}/{reader.Length}");
                }
            }

            errorStatusCode = HttpStatusCode.OK;

            var login = _connectionStateManager.GetLoginByToken(request.Token);

            _addNewStateExecutor.Handler(login, rawPath);
            _historyService.AddNewEvent(login, rawPath, "Download");

            errorMessage = null;
            return true;
        }

        private string GetRawPath(string[] names)
        {
            var sb = new StringBuilder();
            foreach (var path in names) sb.Append($"{path}{Path.DirectorySeparatorChar}");

            var rawPath = sb.ToString();
            return rawPath.Substring(0, rawPath.Length - 1);
        }

        #endregion
    }

    internal class FileCrutch
    {
        [JsonProperty(PropertyName = "path")] public string[] FileName { get; set; }
    }
}