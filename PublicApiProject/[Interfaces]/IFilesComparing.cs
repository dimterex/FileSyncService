using System.Collections.Generic;
using FileSystemProject;
using SdkProject.Api.Sync;

namespace PublicProject._Interfaces_
{
    public interface IFilesComparing
    {
        void Apply(SyncStateFilesResponse response, IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase, IList<FileInfoModel> filesFromServer);
    }
}