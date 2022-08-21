using Newtonsoft.Json;

namespace SdkProject.Api.Sync.Common
{
    public class FileItem
    {
        [JsonProperty(PropertyName = "path")] public string[] Path { get; set; }

        [JsonProperty(PropertyName = "size")] public long Size { get; set; }
    }
}