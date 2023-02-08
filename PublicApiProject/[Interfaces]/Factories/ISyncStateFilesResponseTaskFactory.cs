namespace PublicProject._Interfaces_.Factories
{
    using Logic;

    using Modules;

    using SdkProject.Api.Sync;

    public interface ISyncStateFilesResponseTaskFactory
    {
        SyncStateFilesResponseTask Create(string login, string token, SyncStateFilesBodyRequest bodyRequest, HttpRequestEventModel e);
    }
}
