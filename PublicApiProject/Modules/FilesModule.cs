using System;
using System.IO;
using System.Net;
using System.Text;
using Core.Publisher;
using FileSystemProject;
using Newtonsoft.Json;
using NLog;
using PublicProject._Interfaces_;
using SdkProject.Api.Files;
using ServicesApi.Database.States;
using ServicesApi.Telegram;
using WebSocketSharp.Server;

namespace PublicProject.Modules
{
    public class FilesModule : BaseApiModule
    {

        #region Constants

        private const string UPLOAD_REQUEST_NAME = "upload";

        private const string DOWNLOAD_REQUEST_NAME = "download";
        
        private const int READ_FILE_BUFFER_SIZE = 81920;

        #endregion

        #region Fields

        private readonly Logger _logger;
        private readonly IConnectionStateManager _connectionStateManager;
        private readonly PublisherController _publisherController;

        #endregion

        #region Constructors


        public FilesModule(IConnectionStateManager connectionStateManager,
            PublisherController publisherController) : base("files", new Version(0, 1))
        {
            _logger = LogManager.GetCurrentClassLogger();
            _connectionStateManager = connectionStateManager;
            _publisherController = publisherController;
        }

        #endregion

        #region Methods

        protected override void OnInitialize()
        {
            // Регистрация обработчиков для REST-запросов:
            RegisterPostRequest<UploadRequest>(UPLOAD_REQUEST_NAME, HandleUploadRequest);
            RegisterGetRequest<DownloadRequest>(DOWNLOAD_REQUEST_NAME, HandleDownloadRequest);
        }

        private void HandleUploadRequest(UploadRequest request, HttpRequestEventArgs e)
        {
            _logger.Debug(() => $"Upload {request.FileName}");
            
            var login = _connectionStateManager.GetLoginByToken(request.Token);
    
            
            bool isValidRequest = HandleUploadRequest(
                login,
                request.FileName,
                e.Request.InputStream,
                e.Request.ContentLength64,
                out UploadResponse response,
                out HttpStatusCode errorStatusCode,
                out string errorMessage);

            if (!isValidRequest)
            {
                e.Response.StatusCode = (int)errorStatusCode;
                _logger.Warn(() => errorMessage);
            }

            _publisherController.Send(new TelegramMessage()
            {
                Message = $"Added from {login}: {request.FileName}"
            });
            
            e.Response.SendChunked = true;
            using (var streamWriter = new StreamWriter(e.Response.OutputStream, Encoding.UTF8))
            {
                var serializer = JsonSerializer.Create(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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

        private void HandleDownloadRequest(DownloadRequest request, HttpRequestEventArgs e)
        {
            e.Response.SendChunked = true;

            _logger.Debug(() => $"Download {request.FilePath}");
            if (HandleDownloadRequest(request, e.Response.OutputStream, out HttpStatusCode errorStatusCode, out string errorMessage))
                return;
            
            e.Response.StatusCode = (int)errorStatusCode;
            _logger.Trace(() => errorMessage);
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
            response.FileId = Guid.NewGuid().ToString();

            errorStatusCode = HttpStatusCode.OK;
            
            _publisherController.Send(new AddNewState()
            {
                Login = login,
                FilePath = filePath
            });
            
            errorMessage = null;

            return true;
        }
        
        private bool HandleDownloadRequest(DownloadRequest request, Stream stream, out HttpStatusCode errorStatusCode, out string errorMessage)
        {
            errorStatusCode = HttpStatusCode.Forbidden;

            if (string.IsNullOrEmpty(request.FilePath) || !File.Exists(request.FilePath))
            {
                errorStatusCode = HttpStatusCode.NotFound;
                errorMessage = $"File {request.FilePath} is not exist!";
                return false;
            }

            // В реализации `ResponseStream` в качестве буфера используется `MemoryStream`,
            // в который идёт запись до тех пор, пока поток не будет закрыт или не будет явно вызван метод `ResponseStream.Flush()`.
            // Чтобы при чтении больших файлов не уходить в `OutOfMemory`, читаем файл порционно.
            // read stream from file
            using (Stream reader = File.OpenRead(request.FilePath))
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
            
            _publisherController.Send(new AddNewState()
            {
                Login = login,
                FilePath = request.FilePath
            });
            
            errorMessage = null;
            return true;
        }
        
        #endregion
    }
}