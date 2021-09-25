using System;
using System.IO;
using System.Linq;
using System.Net;
using NLog;
using Service.Api.Message.Attach;
using Service.DataBase;
using Service.DataBase.Dto;
using Service.Transport;

namespace Service.Api
{
     public class AttachmentService
    {
        private readonly SettingsManager _settingsManager;
        private readonly ConnectionStateManager _connectionStateManager;

        #region Constants

        private const int CREATE_FILE_WAITING_INTERVAL = 30000;
        private const int READ_FILE_BUFFER_SIZE = 81920;

        #endregion Constants

        #region Fields

        private readonly Logger _logger;

        #endregion Fields

        #region Constructor

        public AttachmentService(SettingsManager settingsManager, ConnectionStateManager connectionStateManager)
        {
            _settingsManager = settingsManager;
            _connectionStateManager = connectionStateManager;
            _logger = LogManager.GetLogger(nameof(AttachmentService));
            _logger.Info("init");
        }

        #endregion Constructor

        #region Methods

       
        public bool HandleUploadRequest(
            UploadRequest request,
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
                response.Result = AttachmentOperationResult.UnexpectedError;
                errorMessage = "Unable to determine file size";
                return false;
            }

            using (var fileStream = File.Create(request.FileName))
            {
                stream.CopyTo(fileStream);
            }
            
            response.Result = AttachmentOperationResult.Success;
            response.FileId = Guid.NewGuid().ToString();

            errorStatusCode = HttpStatusCode.OK;
            
            var login = _connectionStateManager.GetLoginByToken(request.Token);
            _settingsManager.SaveSyncState(login, request.FileName);
            
            errorMessage = null;

            return true;
        }

        public bool HandleDownloadRequest(DownloadRequest request, Stream stream, out HttpStatusCode errorStatusCode, out string errorMessage)
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
                }
            }

            errorStatusCode = HttpStatusCode.OK;
            
            var login = _connectionStateManager.GetLoginByToken(request.Token);
            _settingsManager.SaveSyncState(login, request.FilePath);
            
            errorMessage = null;
            return true;
        }

        #endregion Methods
    }
}