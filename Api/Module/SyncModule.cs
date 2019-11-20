namespace Service.Api.Module
{
    using NLog;

    using Service.Api.Message;
    using Service.Api.Message.Common;
    using Service.Api.Message.Sync;
    using Service.Transport;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SyncModule
    {
        private readonly FileManager _fileManager;
        private readonly ApiController _apiController;
        private readonly ILogger _logger;

        public SyncModule(ApiController apiController, FileManager fileManager)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _fileManager = fileManager;
            _apiController = apiController;

            _apiController.Configure<SyncFilesRequest>(OnSyncFilesRequest);
        }

        private void OnSyncFilesRequest(SyncFilesRequest fileAction, IClient client)
        {
            var root_files = _fileManager.GetFileList();
            var not_exist_in_server = new List<BaseFileInfo>();
            var not_exist_in_client = new List<BaseFileInfo>();
            _fileManager.CompairFolders(not_exist_in_server, fileAction.Files, root_files);
            _fileManager.CompairFolders(not_exist_in_client, root_files, fileAction.Files);

            SendFilesAddResponce(not_exist_in_client, client);
            SendFilesRemoveResponce(not_exist_in_server, client);
        }

        private void SendFilesAddResponce(List<BaseFileInfo> not_exist_in_client, IClient client)
        {
            foreach (BaseFileInfo baseFileInfo in not_exist_in_client)
            {
                if (!client.IsConnected)
                    return;

                int default_size = 1024;

                var realPath = _fileManager.GetRealPath(baseFileInfo);

                FileInfo fileInfo = new FileInfo(realPath);

                var fileAddResponce = new FileAddResponce();
                fileAddResponce.FileName = fileInfo.Name;
                fileAddResponce.FilePath.AddRange(_fileManager.RemoveRootPath(fileInfo));
                fileAddResponce.Count = (long)Math.Ceiling((double)fileInfo.Length / default_size);

                using (Stream source = File.OpenRead(realPath))
                {
                    int position;

                    byte[] buffer = new byte[default_size];

                    while ((position = source.Read(buffer, 0, buffer.Length)) > 0 && client.IsConnected)
                    {
                        var size = fileInfo.Length - (fileInfo.Length - position);
                        if (size < default_size)
                            default_size = (int)size;

                        var tmp = buffer.Take(default_size);
                        string base64EncodedExternalAccount = Convert.ToBase64String(tmp.ToArray());
                        fileAddResponce.Stream = base64EncodedExternalAccount;
                        fileAddResponce.Current++;
                        _apiController.Send(client, fileAddResponce);
                    }
                }
            }
        }

        private void SendFilesRemoveResponce(List<BaseFileInfo> not_exist_in_server, IClient client)
        {
            foreach (BaseFileInfo baseFileInfo in not_exist_in_server)
            {
                if (!client.IsConnected)
                    return;

                var fileRemoveResponce = new FileRemoveResponce();
                fileRemoveResponce.FileName = baseFileInfo.FileName;
                fileRemoveResponce.FilePath.AddRange(baseFileInfo.FilePath);
                _apiController.Send(client, fileRemoveResponce); 
            }
        }
    }
}
