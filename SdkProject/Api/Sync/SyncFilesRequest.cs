using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SdkProject.Api.Sync
{

    public class SyncFilesRequest : IMessage
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}
