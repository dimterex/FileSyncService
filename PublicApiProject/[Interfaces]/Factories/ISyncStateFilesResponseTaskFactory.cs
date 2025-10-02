namespace PublicProject._Interfaces_.Factories
{
    using Core.WebServiceBase.Models;

    using Logic;

    using SdkProject.Api.Sync;

    public interface ISyncStateFilesResponseTaskFactory
    {
        SyncStateFilesResponseTask Create(string login, string token, SyncStateFilesBodyRequest bodyRequest, HttpRequestEventModel e);
    }
}
