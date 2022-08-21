using System.Collections.Generic;
using System.Linq;
using FileSystemProject;
using PublicProject._Interfaces_;
using PublicProject.Helper;
using SdkProject.Api.Sync;

namespace PublicProject.Logic.Comparing
{
    public class ServerAddFiles : IFilesComparing
    {
        public void Apply(SyncStateFilesResponse response, IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase, IList<FileInfoModel> filesFromServer)
        {
            foreach (var fileFromDevice in deviceFolderFiles)
            {
                var databasePath = filesFromDataBase.FirstOrDefault(x => x == fileFromDevice.Path);
                if (databasePath != null) continue;

                var serverPath = filesFromServer.FirstOrDefault(x => x.Path == fileFromDevice.Path);
                if (serverPath != null) continue;
                var fileUploadRequest = new FileUploadRequest();
                fileUploadRequest.FileName = PathHelper.GetListOfPath(fileFromDevice.Path);
                response.UploadedFiles.Add(fileUploadRequest);
            }
        }
    }
}