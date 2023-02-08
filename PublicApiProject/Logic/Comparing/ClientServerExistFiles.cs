namespace PublicProject.Logic.Comparing
{
    using System.Collections.Generic;
    using System.Linq;

    using _Interfaces_;

    using FileSystemProject;

    using Helper;

    using SdkProject.Api.Sync;

    /// <summary>
    /// Database add files
    /// </summary>
    public class ClientServerExistFiles : IFilesComparing
    {
        public void Apply(
            SyncStateFilesResponse response,
            IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase,
            IList<FileInfoModel> filesFromServer)
        {
            foreach (FileInfoModel fileFromDevice in deviceFolderFiles)
            {
                FileInfoModel serverPath = filesFromServer.FirstOrDefault(x => x.Path == fileFromDevice.Path);
                if (serverPath == null) continue;

                string databaseFile = filesFromDataBase.FirstOrDefault(x => x == fileFromDevice.Path);
                if (databaseFile != null)
                    continue;

                var fileUpdatedResponse = new FileDataBaseAddResponse();
                fileUpdatedResponse.FileName = PathHelper.GetListOfPath(fileFromDevice.Path);
                response.DatabaseAddedFiles.Add(fileUpdatedResponse);
            }
        }
    }
}
