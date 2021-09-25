using Service.Api.Interfaces;
using Service.Api.Module.Common;

namespace Service.Api.Module
{
    using NLog;

    using Service.Api.Message;
    using Service.Api.Message.Sync;
    using Service.Transport;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SyncModule : BaseApiModule
    {
        private readonly FileManager _fileManager;
        private readonly SettingsManager _settingsManager;
        private readonly ConnectionStateManager _connectionStateManager;
        private readonly ILogger _logger;

        public SyncModule(FileManager fileManager,
            SettingsManager settingsManager,
            ConnectionStateManager connectionStateManager) : base("sync", new Version(0,1))
        {
            _logger = LogManager.GetCurrentClassLogger();
            _fileManager = fileManager;
            _settingsManager = settingsManager;
            _connectionStateManager = connectionStateManager;
        }

        protected override void OnInitialize()
        {
            RegisterMessage<SyncFilesRequest>(OnSyncFilesRequest);
        }


        private void OnSyncFilesRequest(IClient client, SyncFilesRequest fileAction)
        {
            var login = _connectionStateManager.GetLoginByToken(client.ID);
     
            var syncFiles = _settingsManager.GetFilesForClient(login);
            
            
            var toRemoveInServer = new List<string>();
            _fileManager.CompairFolders(toRemoveInServer, syncFiles, fileAction.Files);
            _settingsManager.RemoveSyncState(login, toRemoveInServer);
            

            var toDownloadInServer = new List<string>();
            _fileManager.CompairFolders(toDownloadInServer, fileAction.Files, syncFiles);
           
            
            var folders = _settingsManager.GetFoldersForClient(login);
            
            var notExistInServer = new List<string>();
            var notExistInClient = new List<string>();
            
            foreach (var folder in folders)
            {
                var rootFiles = _fileManager.GetFileList(folder);
               
                _fileManager.CompairFolders(notExistInServer, fileAction.Files, rootFiles);
                _fileManager.CompairFolders(notExistInClient, rootFiles, fileAction.Files);
            }
            
            var needToAdd = notExistInClient.Where(x => !syncFiles.Contains(x)).ToList();
            var needToRemove = notExistInServer.Where(x => syncFiles.Contains(x)).ToList();
            
            _fileManager.RemoveFiles(toRemoveInServer);
            SendUploadRequest(toDownloadInServer, client);
            SendFilesAddResponce(needToAdd, client);
            SendFilesRemoveResponce(needToRemove, client);
            _settingsManager.RemoveSyncState(login, needToRemove);
        }

        private void SendUploadRequest(List<string> notExistInServer, IClient client)
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

        private void SendFilesAddResponce(List<string> not_exist_in_client, IClient client)
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

        private void SendFilesRemoveResponce(List<string> not_exist_in_server, IClient client)
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
