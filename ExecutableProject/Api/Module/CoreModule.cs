using DataBaseProject;
using ExecutableProject.Logic;
using FileSystemProject;
using SdkProject.Api.Connection;
using SdkProject.Api.Sync;
using TransportProject;
using WebSocketSharp.Server;

namespace Service.Api.Module
{
    using NLog;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class CoreModule : BaseApiModule
    {
        private readonly IFileManager _fileManager;
        private readonly ISyncTableDataBase _syncTableDataBase;
        private readonly IUserTableDataBase _userTableDataBase;
        private readonly IConnectionStateManager _connectionStateManager;
        private readonly ILogger _logger;
        
        private readonly ServerRemoveFiles _serverRemoveFiles;
        private readonly ClientRemoveFiles _clientRemoveFiles;
        private readonly ServerAddFiles _serverAddFiles;
        private readonly ClientAddFiles _clientAddFiles;
        private readonly ClientUpdateFiles _clientUpdateFiles;

        public CoreModule(IFileManager fileManager,
            ISyncTableDataBase syncTableDataBase,
            IConnectionStateManager connectionStateManager,
            IUserTableDataBase userTableDataBase) : base("core", new Version(0,1))
        {
            _logger = LogManager.GetCurrentClassLogger();
            _fileManager = fileManager;
            _syncTableDataBase = syncTableDataBase;
            _connectionStateManager = connectionStateManager;
            _userTableDataBase = userTableDataBase;
            
            _serverRemoveFiles = new ServerRemoveFiles();
            _clientRemoveFiles = new ClientRemoveFiles();
            _serverAddFiles = new ServerAddFiles();
            _clientAddFiles = new ClientAddFiles();
            _clientUpdateFiles = new ClientUpdateFiles();
        }

        protected override void OnInitialize()
        {
            RegisterPostRequestWithBody<SyncFilesBodyRequest>(OnSyncFilesRequest);
            RegisterPostRequestWithBody<ConnectionRequest>(OnConnectionRequest);
        }

        private void OnConnectionRequest(SyncFilesRequest arg2, ConnectionRequest connectionRequest, HttpRequestEventArgs e)
        {
            try
            {
                var token = Guid.NewGuid();
                _connectionStateManager.Add(connectionRequest.Login, token.ToString());
                var folders =  _userTableDataBase.GetAvailableFolders(connectionRequest.Login);

                var response = new ConnectionResponse()
                {
                    Token = token.ToString()
                };

                foreach (var folder in folders)
                {
                    var sharedFolder = new SharedFolder();
                    sharedFolder.Files.AddRange(GetListOfPath(folder));
                    response.Shared_folders.Add(sharedFolder);
                }
               
                ApiController.SendResponse(e, response);
            }
            catch (Exception exception)
            {
                _logger.Error(() => $"{exception}");
                throw;
            }
        }

        private void OnSyncFilesRequest(SyncFilesRequest fileAction, SyncFilesBodyRequest bodyRequest, HttpRequestEventArgs e)
        {
            var login = _connectionStateManager.GetLoginByToken(fileAction.Token);
            var folders = _userTableDataBase.GetAvailableFolders(login);
            var databaseFiles = _syncTableDataBase.GetSyncStates(login);

            var serverFiles = new List<FileInfoModel>();
            foreach (var folder in folders)
            {
                var rootFiles = _fileManager.GetFiles(folder);
                serverFiles.AddRange(rootFiles);
            }

            var deviceFiles = bodyRequest.Files.Select(Convert).ToList();

            var resultServerRemoveFiles = _serverRemoveFiles.Get(databaseFiles, deviceFiles, serverFiles);
            _syncTableDataBase.RemoveSyncStates(login, resultServerRemoveFiles.Select(x => x.Path).ToList());
            
            foreach (var filePath in resultServerRemoveFiles.ToList())
            {  
                _fileManager.RemoveFile(filePath.Path);
                RaiseSendMessage($"Remove {filePath.Path}");
            }
            
            var response = new SyncFilesResponse();
            
            var resultClientRemoveFiles = _clientRemoveFiles.Get(databaseFiles, deviceFiles, serverFiles);
            AddFilesRemoveResponse(resultClientRemoveFiles, response);
            
            var resultServerAddFiles = _serverAddFiles.Get(databaseFiles, deviceFiles, serverFiles);
            AddUploadRequest(resultServerAddFiles, response);

            var resultClientAddFiles = _clientAddFiles.Get(databaseFiles, deviceFiles, serverFiles);
            AddFilesAddResponse(resultClientAddFiles, response);

            var resultClientUpdateFiles = _clientUpdateFiles.Get(databaseFiles, deviceFiles, serverFiles);
            AddUpdatedResponse(resultClientUpdateFiles, response);

            ApiController.SendResponse(e, response);
        }

        private FileInfoModel Convert(FileItem fileItem)
        {
            return new FileInfoModel(Path.Combine(fileItem.Path), fileItem.Size);
        }

        private void AddUpdatedResponse(IList<FileInfoModel> fileInfoModels, SyncFilesResponse response)
        {
            foreach (FileInfoModel baseFileInfo in fileInfoModels)
            {
                var fileUpdatedResponse = new FileUpdatedResponse();
                fileUpdatedResponse.FileName = GetListOfPath(baseFileInfo.Path);
                fileUpdatedResponse.Size = baseFileInfo.Size;
                response.UpdatedFiles.Add(fileUpdatedResponse);
            }
        }
        
        private void AddUploadRequest(IList<FileInfoModel> fileInfoModels, SyncFilesResponse response)
        {
            foreach (FileInfoModel baseFileInfo in fileInfoModels)
            {
                var fileUploadRequest = new FileUploadRequest();
                fileUploadRequest.FileName = GetListOfPath(baseFileInfo.Path);
                response.UploadedFiles.Add(fileUploadRequest);
            }
        }

        private void AddFilesAddResponse(IList<FileInfoModel> fileInfoModels, SyncFilesResponse response)
        {
            foreach (FileInfoModel fileInfo in fileInfoModels)
            {
                var fileAddResponse = new FileAddResponse();
                fileAddResponse.FileName = GetListOfPath(fileInfo.Path);
                fileAddResponse.Size = fileInfo.Size;
                response.AddedFiles.Add(fileAddResponse);
            }
        }

        private void AddFilesRemoveResponse(IList<FileInfoModel> fileInfoModels, SyncFilesResponse response)
        {
            foreach (FileInfoModel baseFileInfo in fileInfoModels)
            {
                var fileRemoveResponse = new FileRemoveResponse();
                fileRemoveResponse.FileName = GetListOfPath(baseFileInfo.Path);
                response.RemovedFiles.Add(fileRemoveResponse);
            }
        }

        private string[] GetListOfPath(string path)
        {
            return path.Split(Path.DirectorySeparatorChar);
        }
    }
}
