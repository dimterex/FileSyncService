using System.Collections.Generic;
using Newtonsoft.Json;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Connection
{
    [SdkApiMessage("ConnectionResponse")]
    public class ConnectionResponse : ISdkMessage
    {
        public ConnectionResponse()
        {
            Shared_folders = new List<SharedFolder>();
        }

        [JsonProperty(PropertyName = "token")] public string Token { get; set; }

        [JsonProperty(PropertyName = "shared_folders")]
        public List<SharedFolder> Shared_folders { get; set; }
    }
}