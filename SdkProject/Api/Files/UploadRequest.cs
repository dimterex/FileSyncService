using Newtonsoft.Json;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Files
{
    public class UploadRequest : ISdkMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "token")] 
        public string Token { get; set; }
    }
}