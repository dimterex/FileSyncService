using System.Collections.Generic;
using Newtonsoft.Json;

namespace SdkProject.Api.Sync
{
    [ApiMessage("SyncFilesRequest")]
    public class SyncFilesBodyRequest : IMessage
    {
        [JsonProperty(PropertyName = "files")]
        public List<string> Files { get; set; }

        public SyncFilesBodyRequest()
        {
            Files = new List<string>();
        }
    }
}