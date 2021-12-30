using System.Collections.Generic;
using Newtonsoft.Json;

namespace SdkProject.Api.Connection
{
    [ApiMessage("ConnectionResponse")]
    public class ConnectionResponse : IMessage
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
        
        [JsonProperty(PropertyName = "shared_folders")]
        public List<SharedFolder> Shared_folders { get; set; }

        public ConnectionResponse()
        {
            Shared_folders = new List<SharedFolder>();
        }
    }
}