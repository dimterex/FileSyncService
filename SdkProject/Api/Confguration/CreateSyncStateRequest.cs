using Newtonsoft.Json;

namespace SdkProject.Api.Confguration
{
    [ApiMessage("CreateSyncStateRequest")]
    public class CreateSyncStateRequest : IMessage
    {
        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }
        
        
        [JsonProperty(PropertyName = "sync_files")]
        public string[] SyncFiles { get; set; }
    }
}