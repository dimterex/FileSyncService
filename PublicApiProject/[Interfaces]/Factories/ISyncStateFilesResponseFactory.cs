using System.Collections.Generic;
using FileSystemProject;
using SdkProject.Api.Sync;
using SdkProject.Api.Sync.Common;

namespace PublicProject._Interfaces_.Factories
{
    public interface ISyncStateFilesResponseFactory
    {
        SyncStateFilesResponse Build(IList<string> databaseFiles, IList<FolderItem> deviceFolders,
            IList<DictionaryModel> serverFiles);
    }
}