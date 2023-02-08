namespace PublicProject.Logic.Comparing
{
    using System.Collections.Generic;
    using System.Linq;

    using _Interfaces_;

    using FileSystemProject;

    using Helper;

    using SdkProject.Api.Sync;

    /// <summary>
    /// Device remove files
    /// </summary>
    public class ClientRemoveFiles : IFilesComparing
    {
        public void Apply(
            SyncStateFilesResponse response,
            IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase,
            IList<FileInfoModel> filesFromServer)
        {
            foreach (string fileFromDataBase in filesFromDataBase)
            {
                FileInfoModel deviceFile = deviceFolderFiles.FirstOrDefault(x => x.Path == fileFromDataBase);
                if (deviceFile == null)
                    continue;

                FileInfoModel serverPath = filesFromServer.FirstOrDefault(x => x.Path == fileFromDataBase);
                if (serverPath == null)
                {
                    var fileRemoveResponse = new FileRemoveResponse();
                    fileRemoveResponse.FileName = PathHelper.GetListOfPath(deviceFile.Path);
                    response.RemovedFiles.Add(fileRemoveResponse);
                }
            }
        }
    }
}
