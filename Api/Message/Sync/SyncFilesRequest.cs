namespace Service.Api.Message
{
    using Newtonsoft.Json;

    using Service.Api.Interfaces;
    using Service.Api.Message.Common;
    using Service.Attribute.Api;

    using System.Collections.Generic;

    [ApiMessage("SyncFilesRequest")]
    public class SyncFilesRequest : IMessage
    {
        [JsonProperty(PropertyName = "files")]
        public List<BaseFileInfo> Files { get; set; }

        public SyncFilesRequest()
        {
            Files = new List<BaseFileInfo>();
        }
    }
}
