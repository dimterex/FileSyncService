namespace PublicProject.Logic.Comparing
{
    using System.Collections.Generic;
    using System.Linq;

    using _Interfaces_;

    using FileSystemProject;

    using Helper;

    using SdkProject.Api.Sync;

    /// <summary>
    /// Device update files.
    /// File on server side was modified. We need to update it on device.
    /// </summary>
    public class ClientUpdateFiles : IFilesComparing
    {
        public void Apply(
            SyncStateFilesResponse response,
            IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase,
            IList<FileInfoModel> filesFromServer)
        {
            foreach (FileInfoModel serverFileInfoModel in filesFromServer)
            {
                if (NotExistOnDataBase(filesFromDataBase, serverFileInfoModel.Path))
                    continue;

                FileInfoModel fileFromDevice = deviceFolderFiles.FirstOrDefault(x => x.Path == serverFileInfoModel.Path);

                if (fileFromDevice == null)
                    continue;

                if (serverFileInfoModel.Size == fileFromDevice.Size)
                    continue;

                if (serverFileInfoModel.LastWriteTimeUtc < fileFromDevice.LastWriteTimeUtc)
                    continue;

                var fileUpdatedResponse = new FileUpdatedResponse();
                fileUpdatedResponse.FileName = PathHelper.GetListOfPath(fileFromDevice.Path);
                fileUpdatedResponse.Size = fileFromDevice.Size;
                response.UpdatedFiles.Add(fileUpdatedResponse);
            }
        }

        private bool NotExistOnDataBase(IList<string> filesFromDataBase, string path)
        {
            return filesFromDataBase.All(x => x != path);
        }
    }
}
