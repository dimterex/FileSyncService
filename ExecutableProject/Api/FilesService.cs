using System;
using System.IO;
using System.Net;
using DataBaseProject;
using NLog;
using SdkProject.Api.Files;
using TransportProject;

namespace Service.Api
{
     public class FilesService
    {
        private readonly IConnectionStateManager _connectionStateManager;
        private readonly ISyncTableDataBase _syncTableDataBase;

        #region Constants

        private const int CREATE_FILE_WAITING_INTERVAL = 30000;
        private const int READ_FILE_BUFFER_SIZE = 81920;

        #endregion Constants

        #region Fields

        private readonly Logger _logger;

        #endregion Fields

        #region Constructor

        public FilesService(IConnectionStateManager connectionStateManager,
                                ISyncTableDataBase syncTableDataBase)
        {
            _connectionStateManager = connectionStateManager;
            _syncTableDataBase = syncTableDataBase;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion Constructor

        #region Methods

       
        public bool HandleUploadRequest(
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
            
            _syncTableDataBase.AddState(login, filePath);
            
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
                    _logger.Trace(() => $"Sending {request.FilePath} ${count}/{reader.Length}");
                }
            }

            errorStatusCode = HttpStatusCode.OK;
            
            var login = _connectionStateManager.GetLoginByToken(request.Token);
            
            _syncTableDataBase.AddState(login, request.FilePath);
            
            errorMessage = null;
            return true;
        }

        #endregion Methods
    }
}