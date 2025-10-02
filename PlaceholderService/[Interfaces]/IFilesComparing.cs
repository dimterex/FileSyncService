namespace PlaceholderService._Interfaces_
{
    using System.Collections.Generic;

    using FileSystemProject;

    using SdkProject.Api.Sync;

    public interface IFilesComparing
    {
        void Apply(
            SyncStateFilesResponse response,
            IList<FileInfoModel> deviceFolderFiles,
            IList<string> filesFromDataBase,
            IList<FileInfoModel> filesFromServer);
    }
}
