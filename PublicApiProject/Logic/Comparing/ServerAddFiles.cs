namespace PublicProject.Logic.Comparing
{
    using System.Collections.Generic;
    using System.Linq;

    using _Interfaces_;

    using FileSystemProject;

    using Helper;

    using SdkProject.Api.Sync;

    /// <summary>
    /// Server update files.
    /// File on device side was modified. We need to update it on server.
    /// </summary>
    public class ServerAddFiles : IFilesComparing
    {
        private readonly IFileManager _fileManager;

        public ServerAddFiles(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void Apply(
            SyncStateFilesResponse response,
            IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase,
            IList<FileInfoModel> filesFromServer)
        {
            foreach (FileInfoModel fileFromDevice in deviceFolderFiles)
            {
                string databasePath = filesFromDataBase.FirstOrDefault(x => x == fileFromDevice.Path);
                if (databasePath != null)
                {
                    if (_fileManager.TryGetFileInfo(databasePath, out FileInfoModel fileInfo))
                    {
                        if (fileInfo.Size == fileFromDevice.Size)
                            continue;

                        if (fileInfo.LastWriteTimeUtc > fileFromDevice.LastWriteTimeUtc)
                            continue;
                    }
                    else
                    {
                        continue;
                    }
                }

                FileInfoModel serverPath = filesFromServer.FirstOrDefault(x => x.Path == fileFromDevice.Path);
                if (serverPath != null)
                {
                    if (serverPath.Size == fileFromDevice.Size)
                        continue;

                    if (serverPath.LastWriteTimeUtc > fileFromDevice.LastWriteTimeUtc)
                        continue;
                }

                var fileUploadRequest = new FileUploadResponse();
                fileUploadRequest.FileName = PathHelper.GetListOfPath(fileFromDevice.Path);
                response.UploadedFiles.Add(fileUploadRequest);
            }
        }
    }
}
