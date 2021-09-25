using Newtonsoft.Json;
using Service.Api.Interfaces;
using Service.Attribute.Api;

namespace Service.Api.Message.Connection
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