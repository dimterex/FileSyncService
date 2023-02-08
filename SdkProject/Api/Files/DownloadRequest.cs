namespace SdkProject.Api.Files
{
    using _Interfaces_;

    using Newtonsoft.Json;

    public class DownloadRequest : ISdkMessage
    {
        [JsonProperty(PropertyName = "file_id")]
        public string FilePath { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}
