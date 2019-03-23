using Newtonsoft.Json;
using System.Collections.Generic;

namespace Service.Api.Message
{
    public class SyncFilesRequest
    {
        [JsonProperty(PropertyName = "files")]
        public List<BaseFileInfo> Files { get; set; }

        public SyncFilesRequest()
        {
            Files = new List<BaseFileInfo>();
        }
    }
}
