namespace Service.Api.Message
{
    using Newtonsoft.Json;

    using Service.Api.Interfaces;
    using Service.Attribute.Api;

    using System.Collections.Generic;

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
