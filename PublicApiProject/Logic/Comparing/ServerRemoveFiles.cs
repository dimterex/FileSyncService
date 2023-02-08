namespace PublicProject.Logic.Comparing
{
    using System.Collections.Generic;
    using System.Linq;

    using _Interfaces_;

    using FileSystemProject;

    using Helper;

    using SdkProject.Api.Sync;

    /// <summary>
    /// Remove file from server, because nobody have it on devices.
    /// </summary>
    public class ServerRemoveFiles : IFilesComparing
    {
        public void Apply(
            SyncStateFilesResponse response,
            IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase,
            IList<FileInfoModel> filesFromServer)
        {
            foreach (string fileFromDataBase in filesFromDataBase)
            {
                foreach (FileInfoModel fileFromServer in filesFromServer)
                {
                    if (fileFromServer.Path != fileFromDataBase)
                        continue;

                    FileInfoModel devicePath = deviceFolderFiles.FirstOrDefault(x => x.Path == fileFromDataBase);
                    if (devicePath != null)
                        continue;

                    var fileUpdatedResponse = new FileServerRemovedResponse();
                    fileUpdatedResponse.FileName = PathHelper.GetListOfPath(fileFromServer.Path);
                    response.ServerRemovedFiles.Add(fileUpdatedResponse);
                }
            }
        }
    }
}
