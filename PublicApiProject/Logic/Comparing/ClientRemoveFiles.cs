using System.Collections.Generic;
using System.Linq;
using FileSystemProject;
using PublicProject._Interfaces_;
using PublicProject.Helper;
using SdkProject.Api.Sync;

namespace PublicProject.Logic.Comparing
{
    public class ClientRemoveFiles : IFilesComparing
    {
        public void Apply(SyncStateFilesResponse response, IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase, IList<FileInfoModel> filesFromServer)
        {
            foreach (var fileFromDataBase in filesFromDataBase)
            {
                var deviceFile = deviceFolderFiles.FirstOrDefault(x => x.Path == fileFromDataBase);
                if (deviceFile == null)
                    continue;

                var serverPath = filesFromServer.FirstOrDefault(x => x.Path == fileFromDataBase);
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