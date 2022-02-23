using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Publisher;
using FileSystemProject;
using NLog;
using PublicProject._Interfaces_;
using PublicProject.Logic;
using SdkProject.Api.Connection;
using SdkProject.Api.Sync;
using ServicesApi.Database.States;
using ServicesApi.Database.Users;
using ServicesApi.Telegram;
using WebSocketSharp.Server;

namespace PublicProject.Modules
{
   public class CoreModule : BaseApiModule
    {
        private readonly IFileManager _fileManager;
        private readonly IConnectionStateManager _connectionStateManager;
        private readonly PublisherController _publisherController;
        private readonly ILogger _logger;
        
        private readonly ServerRemoveFiles _serverRemoveFiles;
        private readonly ClientRemoveFiles _clientRemoveFiles;
        private readonly ServerAddFiles _serverAddFiles;
        private readonly ClientAddFiles _clientAddFiles;
        private readonly ClientUpdateFiles _clientUpdateFiles;
        private readonly ClientServerExistFiles _clientServerExistFiles;

        public CoreModule(IFileManager fileManager,
            IConnectionStateManager connectionStateManager,
            PublisherController publisherController) : base("core", new Version(0,1))
        {
            _logger = LogManager.GetCurrentClassLogger();
            _fileManager = fileManager;
            _connectionStateManager = connectionStateManager;
            _publisherController = publisherController;
            
            _serverRemoveFiles = new ServerRemoveFiles();
            _clientRemoveFiles = new ClientRemoveFiles();
            _serverAddFiles = new ServerAddFiles();
            _clientAddFiles = new ClientAddFiles();
            _clientUpdateFiles = new ClientUpdateFiles();
            _clientServerExistFiles = new ClientServerExistFiles();
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

                var folders =  GetAvailableFolders(connectionRequest.Login);

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

        private IList<string> GetSyncStates(string login)
        {
            var response = _publisherController.SendWithResponse(
                new SyncStatesRequest()
                {
                    Login = login,
                });

            if (response is SyncStatesResponse statesResponse)
                return statesResponse.FilePaths.ToList();

            return new List<string>();
        }
        
        private IList<string> GetAvailableFolders(string login)
        {
            var response = _publisherController.SendWithResponse(new AvailableFoldersForUserRequest()
                {
                    Login = login,
                });

            if (response is AvailableFoldersForUserResponse availableFoldersForUserResponse)
                return availableFoldersForUserResponse.FilePaths.ToList();

            return new List<string>();
        }

        private void OnSyncFilesRequest(SyncFilesRequest fileAction, SyncFilesBodyRequest bodyRequest, HttpRequestEventArgs e)
        {
            var login = _connectionStateManager.GetLoginByToken(fileAction.Token);
            if (string.IsNullOrEmpty(login))
            {
                ApiController.SetErrorResponse(e);
                return;
            }
            
            var folders = GetAvailableFolders(login);
            var databaseFiles = GetSyncStates(login);

            var serverFiles = new List<FileInfoModel>();
            foreach (var folder in folders)
            {
                var rootFiles = _fileManager.GetFiles(folder);
                serverFiles.AddRange(rootFiles.Select(x => new FileInfoModel(x.Path, x.Size)));
            }

            var deviceFiles = bodyRequest.Files.Select(Convert).ToList();

            var resultServerRemoveFiles = _serverRemoveFiles.Get(databaseFiles, deviceFiles, serverFiles);
           
            
            var response = new SyncFilesResponse();
            
            var resultClientRemoveFiles = _clientRemoveFiles.Get(databaseFiles, deviceFiles, serverFiles);
            AddFilesRemoveResponse(resultClientRemoveFiles, response);
            
            var resultServerAddFiles = _serverAddFiles.Get(databaseFiles, deviceFiles, serverFiles);
            AddUploadRequest(resultServerAddFiles, response);

            var resultClientAddFiles = _clientAddFiles.Get(databaseFiles, deviceFiles, serverFiles);
            AddFilesAddResponse(resultClientAddFiles, response);

            var resultClientUpdateFiles = _clientUpdateFiles.Get(databaseFiles, deviceFiles, serverFiles);
            AddUpdatedResponse(resultClientUpdateFiles, response);
            
            var clientServerExistFiles = _clientServerExistFiles.Get(databaseFiles, deviceFiles, serverFiles);
            
            _publisherController.Send(new AddNewStates()
            {
                Login = login,
                FilePaths = clientServerExistFiles.Select(x => x.Path).ToArray(),
            });
            
            _publisherController.Send(new RemoveSyncStates()
            {
                Login = login,
                FilePaths = resultServerRemoveFiles.Select(x => x.Path).ToArray(),
            });
            
            foreach (var filePath in resultServerRemoveFiles.ToList())
            {  
                _fileManager.RemoveFile(filePath.Path);
                _publisherController.Send(new TelegramMessage()
                {
                    Message = $"Remove {filePath.Path}"
                });
            }
            
            ApiController.SendResponse(e, response);
        }

        private FileInfoModel Convert(FileItem fileItem)
        {
            var sb = new StringBuilder();
            foreach (var path in fileItem.Path)
            {
                sb.Append($"{path}{Path.DirectorySeparatorChar}");
            }

            var rawPath = sb.ToString();
            return new FileInfoModel(rawPath.Substring(0, rawPath.Length - 1), fileItem.Size);
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