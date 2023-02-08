namespace ServicesApi.Configuration
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class AvailableFolder
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "available_folder_action")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AvailableFolderAction AvailableFolderAction { get; set; }
    }
}
