using Newtonsoft.Json;

namespace SdkProject.Api.Sync
{
    public class FileItem
    {
        [JsonProperty(PropertyName = "path")]
        public string[] Path { get; set; }
        
        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }
    }
}