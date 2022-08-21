using PublicProject.Logic;
using PublicProject.Modules;
using SdkProject.Api.Sync;

namespace PublicProject._Interfaces_.Factories
{
    public interface ISyncStateFilesResponseTaskFactory
    {
        SyncStateFilesResponseTask Create(string login, string token, SyncStateFilesBodyRequest bodyRequest,
            HttpRequestEventModel e);
    }
}