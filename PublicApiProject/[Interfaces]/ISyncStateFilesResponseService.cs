namespace PublicProject._Interfaces_
{
    using SdkProject.Api.Sync;

    public interface ISyncStateFilesResponseService
    {
        SyncStateFilesResponse GetResponse(string fileActionToken);
        void Remove(string login, string token);
        void Add(string login, string token, SyncStateFilesResponse response);
    }
}
