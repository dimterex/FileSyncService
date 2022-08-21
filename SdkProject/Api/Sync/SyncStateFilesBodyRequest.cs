using System.Collections.Generic;
using Newtonsoft.Json;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;
using SdkProject.Api.Sync.Common;

namespace SdkProject.Api.Sync
{
    [SdkApiMessage("SyncStateFilesRequest")]
    public class SyncStateFilesBodyRequest : ISdkMessage
    {
        public SyncStateFilesBodyRequest()
        {
            Folders = new List<FolderItem>();
        }

        [JsonProperty(PropertyName = "folders")]
        public List<FolderItem> Folders { get; set; }
    }
}