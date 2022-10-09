using System;
using System.Collections.Generic;
using System.Linq;
using FileSystemProject;
using PublicProject._Interfaces_;
using PublicProject.Helper;
using SdkProject.Api.Sync;

namespace PublicProject.Logic.Comparing
{
    public class ClientServerExistFiles : IFilesComparing
    {
        public void Apply(SyncStateFilesResponse response, IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase, IList<FileInfoModel> filesFromServer)
        {
            foreach (var fileFromDevice in deviceFolderFiles)
            {
                var serverPath = filesFromServer.FirstOrDefault(x => x.Path == fileFromDevice.Path);
                if (serverPath == null) continue;

                var databaseFile = filesFromDataBase.FirstOrDefault(x => x == fileFromDevice.Path);
                if (databaseFile != null)
                    continue;

                var fileUpdatedResponse = new FileDataBaseAddResponse();
                fileUpdatedResponse.FileName = PathHelper.GetListOfPath(fileFromDevice.Path);
                response.DatabaseAddedFiles.Add(fileUpdatedResponse);
            }
        }
    }
}