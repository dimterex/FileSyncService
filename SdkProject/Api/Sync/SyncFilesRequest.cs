using Newtonsoft.Json;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Sync
{
    public class SyncFilesRequest : ISdkMessage
    {
        [JsonProperty(PropertyName = "token")] public string Token { get; set; }
    }
}