using Newtonsoft.Json;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Files
{
    public class DownloadRequest : ISdkMessage
    {
        [JsonProperty(PropertyName = "file_id")]
        public string FilePath { get; set; }

        [JsonProperty(PropertyName = "token")] public string Token { get; set; }
    }
}