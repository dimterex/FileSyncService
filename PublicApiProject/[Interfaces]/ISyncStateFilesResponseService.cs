using SdkProject.Api.Sync;

namespace PublicProject._Interfaces_
{
    public interface ISyncStateFilesResponseService
    {
        SyncStateFilesResponse GetResponse(string fileActionToken);
        void Remove(string login, string token);
        void Add(string login, string token, SyncStateFilesResponse response);
    }
}