using Newtonsoft.Json;
using Service.Api.Interfaces;
using Service.Attribute.Api;

namespace Service.Api.Message.Connection
{
    [ApiMessage("ConnectionRequest")]
    public class ConnectionRequest : IMessage
    {
        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }
        
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }
}