using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using NLog;
using SdkProject.Api.Files;
using TransportProject;
using WebSocketSharp.Server;

namespace Service.Api.Module
{
    public class FilesApi : BaseApiModule
    {

        #region Constants

        /// <summary>
        /// Имя REST-запроса на загрузку прикрепленного файла.
        /// </summary>
        private const string UPLOAD_REQUEST_NAME = "upload";

        /// <summary>
        /// Имя REST-запроса на скачивание прикрепленного файла.
        /// </summary>
        private const string DOWNLOAD_REQUEST_NAME = "download";

        #endregion

        #region Fields

        private readonly Logger _logger;
        private readonly FilesService _filesService;

        #endregion

        #region Constructors


        public FilesApi(FilesService filesService) : base("files", new Version(0, 1))
        {
            _logger = LogManager.GetCurrentClassLogger();
            _filesService = filesService;
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
            bool isValidRequest = _filesService.HandleUploadRequest(
                request,
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

            if (_filesService.HandleDownloadRequest(request, e.Response.OutputStream, out HttpStatusCode errorStatusCode, out string errorMessage))
                return;

            e.Response.StatusCode = (int)errorStatusCode;
            _logger.Trace(() => errorMessage);
        }
        
        #endregion
    }
}