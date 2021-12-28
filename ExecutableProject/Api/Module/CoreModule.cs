using DataBaseProject;
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
        }

        protected override void OnInitialize()
        {
            RegisterPostRequestWithBody<SyncFilesBodyRequest>(OnSyncFilesRequest);
            RegisterPostRequestWithBody<ConnectionRequest>(OnConnectionRequest);
        }

        private void OnConnectionRequest(IClient client, SyncFilesRequest arg2, ConnectionRequest connectionRequest, HttpRequestEventArgs e)
        {
            try
            {
                var token = Guid.NewGuid();
                _connectionStateManager.Add(connectionRequest.Login, token.ToString());
                var folders =  _userTableDataBase.GetAvailableFolders(connectionRequest.Login);

                ApiController.SendResponse(e, new ConnectionResponse()
                {
                    Shared_folders = folders.ToArray(),
                    Token = token.ToString()
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
           
        }

        private void OnSyncFilesRequest(IClient client, SyncFilesRequest fileAction, SyncFilesBodyRequest bodyRequest, HttpRequestEventArgs e)
        {
            var login = _connectionStateManager.GetLoginByToken(fileAction.Token);
            
            var syncFiles = _syncTableDataBase.GetSyncStates(login);
            var toRemoveInServer = _fileManager.CompairFolders(syncFiles, bodyRequest.Files);
            _syncTableDataBase.RemoveSyncStates(login, toRemoveInServer);
            

            var toDownloadInServer = _fileManager.CompairFolders(bodyRequest.Files, syncFiles);
            
            var folders = _userTableDataBase.GetAvailableFolders(login);
            
            var notExistInServer = new List<string>();
            var notExistInClient = new List<string>();
            
            foreach (var folder in folders)
            {
                var rootFiles = _fileManager.GetFiles(folder);
               
                notExistInServer.AddRange(_fileManager.CompairFolders(bodyRequest.Files, rootFiles));
                notExistInClient.AddRange(_fileManager.CompairFolders(rootFiles, bodyRequest.Files));
            }
            
            var needToAdd = notExistInClient.Where(x => !syncFiles.Contains(x)).ToList();
            var needToRemove = notExistInServer.Where(x => syncFiles.Contains(x)).ToList();
            
            _fileManager.RemoveFiles(toRemoveInServer);

            var response = new SyncFilesResponse();
            
            SendUploadRequest(toDownloadInServer, client, response);
            SendFilesAddResponce(needToAdd, client, response);
            SendFilesRemoveResponce(needToRemove, client, response);
            
            ApiController.SendResponse(e, response);
            _syncTableDataBase.RemoveSyncStates(login, needToRemove);
        }

        private void SendUploadRequest(IList<string> notExistInServer, IClient client, SyncFilesResponse response)
        {
            foreach (string baseFileInfo in notExistInServer)
            {
                var fileUploadRequest = new FileUploadRequest();
                fileUploadRequest.FileName = baseFileInfo;
                response.UploadedFiles.Add(fileUploadRequest);
            }
        }

        private void SendFilesAddResponce(IList<string> not_exist_in_client, IClient client, SyncFilesResponse response)
        {
            foreach (string baseFileInfo in not_exist_in_client)
            {
                FileInfo fileInfo = new FileInfo(baseFileInfo);

                var fileAddResponce = new FileAddResponse();
                fileAddResponce.FileName = fileInfo.FullName;
                fileAddResponce.Size = fileInfo.Length;
                response.AddedFiles.Add(fileAddResponce);
            }
        }

        private void SendFilesRemoveResponce(IList<string> not_exist_in_server, IClient client, SyncFilesResponse response)
        {
            foreach (string baseFileInfo in not_exist_in_server)
            {
                var fileRemoveResponce = new FileRemoveResponse();
                fileRemoveResponce.FileName = baseFileInfo;
                response.RemovedFiles.Add(fileRemoveResponce);
            }
        }
    }
}
