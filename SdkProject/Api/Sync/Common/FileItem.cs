namespace SdkProject.Api.Sync.Common
{
    using Newtonsoft.Json;

    public class FileItem
    {
        [JsonProperty(PropertyName = "path")]
        public string[] Path { get; set; }

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }
    }
}
