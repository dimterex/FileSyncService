using Newtonsoft.Json;

namespace SdkProject.Api.Connection
{
    [ApiMessage("ConnectionResponse")]
    public class ConnectionResponse : IMessage
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
        
        [JsonProperty(PropertyName = "shared_folders")]
        public string[] Shared_folders { get; set; }
    }
}