using System.Collections.Generic;
using System.Linq;
using FileSystemProject;
using PublicProject._Interfaces_;
using PublicProject.Helper;
using SdkProject.Api.Sync;

namespace PublicProject.Logic.Comparing
{
    public class ClientUpdateFiles : IFilesComparing
    {
        public void Apply(SyncStateFilesResponse response, IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase, IList<FileInfoModel> filesFromServer)
        {
            foreach (var serverFileInfoModel in filesFromServer)
            {
                if (NotExistOnDataBase(filesFromDataBase, serverFileInfoModel.Path))
                    continue;

                var fileFromDevice = deviceFolderFiles.FirstOrDefault(x => x.Path == serverFileInfoModel.Path);

                if (fileFromDevice == null)
                    continue;

                if (serverFileInfoModel.Size == fileFromDevice.Size)
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