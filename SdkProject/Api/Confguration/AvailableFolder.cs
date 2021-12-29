using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SdkProject.Api.Confguration
{
    public class AvailableFolder
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }
        
        [JsonProperty(PropertyName = "available_folder_action")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AvailableFolderAction AvailableFolderAction { get; set; }
    }
}