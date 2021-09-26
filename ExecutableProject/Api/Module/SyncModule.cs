using DataBaseProject;
using FileSystemProject;
using SdkProject.Api.Sync;
using TransportProject;

namespace Service.Api.Module
{
    using NLog;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SyncModule : BaseApiModule
    {
        private readonly IFileManager _fileManager;
        private readonly ISyncTableDataBase _syncTableDataBase;
        private readonly IUserTableDataBase _userTableDataBase;
        private readonly IConnectionStateManager _connectionStateManager;
        private readonly ILogger _logger;

        public SyncModule(IFileManager fileManager,
            ISyncTableDataBase syncTableDataBase,
            IConnectionStateManager connectionStateManager,
            IUserTableDataBase userTableDataBase) : base("sync", new Version(0,1))
        {
            _logger = LogManager.GetCurrentClassLogger();
            _fileManager = fileManager;
            _syncTableDataBase = syncTableDataBase;
            _connectionStateManager = connectionStateManager;
            _userTableDataBase = userTableDataBase;
        }

        protected override void OnInitialize()
        {
            RegisterMessage<SyncFilesRequest>(OnSyncFilesRequest);
        }


        private void OnSyncFilesRequest(IClient client, SyncFilesRequest fileAction)
        {
            var login = _connectionStateManager.GetLoginByToken(client.ID);
     
            var syncFiles = _syncTableDataBase.GetSyncStates(login);
            var toRemoveInServer = _fileManager.CompairFolders(syncFiles, fileAction.Files);
            _syncTableDataBase.RemoveSyncStates(login, toRemoveInServer);
            

            var toDownloadInServer = _fileManager.CompairFolders(fileAction.Files, syncFiles);
            
            var folders = _userTableDataBase.GetAvailableFolders(login);
            
            var notExistInServer = new List<string>();
            var notExistInClient = new List<string>();
            
            foreach (var folder in folders)
            {
                var rootFiles = _fileManager.GetFiles(folder);
               
                notExistInServer.AddRange(_fileManager.CompairFolders(fileAction.Files, rootFiles));
                notExistInClient.AddRange(_fileManager.CompairFolders(rootFiles, fileAction.Files));
            }
            
            var needToAdd = notExistInClient.Where(x => !syncFiles.Contains(x)).ToList();
            var needToRemove = notExistInServer.Where(x => syncFiles.Contains(x)).ToList();
            
            _fileManager.RemoveFiles(toRemoveInServer);
            SendUploadRequest(toDownloadInServer, client);
            SendFilesAddResponce(needToAdd, client);
            SendFilesRemoveResponce(needToRemove, client);
            _syncTableDataBase.RemoveSyncStates(login, needToRemove);
        }

        private void SendUploadRequest(IList<string> notExistInServer, IClient client)
        {
            foreach (string baseFileInfo in notExistInServer)
            {
                if (!client.IsConnected)
                    return;

                var fileUploadRequest = new FileUploadRequest();
                fileUploadRequest.FileName = baseFileInfo;
                client.SendMessage(fileUploadRequest);
            }
        }

        private void SendFilesAddResponce(IList<string> not_exist_in_client, IClient client)
        {
            foreach (string baseFileInfo in not_exist_in_client)
            {
                if (!client.IsConnected)
                    return;

                FileInfo fileInfo = new FileInfo(baseFileInfo);

                var fileAddResponce = new FileAddResponce();
                fileAddResponce.FileName = fileInfo.FullName;
                fileAddResponce.Size = fileInfo.Length;

                client.SendMessage(fileAddResponce);
            }
        }

        private void SendFilesRemoveResponce(IList<string> not_exist_in_server, IClient client)
        {
            foreach (string baseFileInfo in not_exist_in_server)
            {
                if (!client.IsConnected)
                    return;

                var fileRemoveResponce = new FileRemoveResponce();
                fileRemoveResponce.FileName = baseFileInfo;
                client.SendMessage(fileRemoveResponce); 
               
            }
        }
    }
}
