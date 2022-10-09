namespace SdkProject.Api.Sync
{
    using System.Collections.Generic;

    using _Attribute_;

    using _Interfaces_;

    using Common;

    using Newtonsoft.Json;

    [SdkApiMessage("sync_state_files_request")]
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
