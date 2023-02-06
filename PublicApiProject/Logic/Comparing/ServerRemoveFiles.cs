using System.Collections.Generic;
using System.Linq;
using FileSystemProject;
using NLog;
using PublicProject._Interfaces_;
using PublicProject.Helper;
using SdkProject.Api.Sync;

namespace PublicProject.Logic.Comparing
{
    public class ServerRemoveFiles : IFilesComparing
    {

        public void Apply(SyncStateFilesResponse response, IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase, IList<FileInfoModel> filesFromServer)
        {
            foreach (var fileFromDataBase in filesFromDataBase)
            {
                foreach (var fileFromServer in filesFromServer)
                {
                    if (fileFromServer.Path != fileFromDataBase)
                        continue;

                    var devicePath = deviceFolderFiles.FirstOrDefault(x => x.Path == fileFromDataBase);
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