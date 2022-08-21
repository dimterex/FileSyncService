using System.Collections.Generic;
using System.Linq;
using FileSystemProject;
using PublicProject._Interfaces_;
using PublicProject.Helper;
using SdkProject.Api.Sync;

namespace PublicProject.Logic.Comparing
{
    public class ClientAddFiles : IFilesComparing
    {
        public void Apply(SyncStateFilesResponse response, IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase, IList<FileInfoModel> filesFromServer)
        {
            foreach (var serverFileInfoModel in filesFromServer)
            {
                if (ExistOnDataBase(filesFromDataBase, serverFileInfoModel.Path))
                    continue;

                if (ExistOnDevice(deviceFolderFiles, serverFileInfoModel.Path))
                    continue;

                var fileAddResponse = new FileAddResponse();
                fileAddResponse.FileName = PathHelper.GetListOfPath(serverFileInfoModel.Path);
                fileAddResponse.Size = serverFileInfoModel.Size;
                response.AddedFiles.Add(fileAddResponse);
            }
        }

        private bool ExistOnDevice(IList<FileInfoModel> files, string path)
        {
            return files.Any(x => path == x.Path);
        }

        private bool ExistOnDataBase(IList<string> filesFromDataBase, string path)
        {
            return filesFromDataBase.Any(x => x == path);
        }
    }
}