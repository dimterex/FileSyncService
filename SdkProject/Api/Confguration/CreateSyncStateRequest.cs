using Newtonsoft.Json;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Confguration
{
    [SdkApiMessage("CreateSyncStateRequest")]
    public class CreateSyncStateRequest : ISdkMessage
    {
        [JsonProperty(PropertyName = "login")] public string Login { get; set; }


        [JsonProperty(PropertyName = "sync_files")]
        public string[] SyncFiles { get; set; }
    }
}