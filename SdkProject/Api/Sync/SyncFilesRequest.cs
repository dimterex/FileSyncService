using System.Collections.Generic;
using Newtonsoft.Json;

namespace SdkProject.Api.Sync
{
    [ApiMessage("SyncFilesRequest")]
    public class SyncFilesRequest : IMessage
    {
        [JsonProperty(PropertyName = "files")]
        public List<string> Files { get; set; }

        public SyncFilesRequest()
        {
            Files = new List<string>();
        }
    }
}
