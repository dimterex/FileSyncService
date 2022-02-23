using System.Collections.Generic;
using Newtonsoft.Json;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Sync
{
    [SdkApiMessage("SyncFilesRequest")]
    public class SyncFilesBodyRequest : ISdkMessage
    {
        [JsonProperty(PropertyName = "files")]
        public List<FileItem> Files { get; set; }

        public SyncFilesBodyRequest()
        {
            Files = new List<FileItem>();
        }
    }
}