using Newtonsoft.Json;

namespace SdkProject.Api.Connection
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