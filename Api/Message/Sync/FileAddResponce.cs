namespace Service.Api.Message.Sync
{
    using Newtonsoft.Json;

    using Service.Api.Message.Common;
    using Service.Attribute.Api;

    [ApiMessage("FileAddResponce")]
    public class FileAddResponce : BaseFileInfo
    {
        [JsonProperty(PropertyName = "stream")]
        public string Stream { get; set; }

        [JsonProperty(PropertyName = "count")]
        public long Count { get; set; }

        [JsonProperty(PropertyName = "current")]
        public long Current { get; set; }

        public FileAddResponce()
        {
            Stream = string.Empty;
            Count = 0;
            Current = 0;
        }
    }
}
