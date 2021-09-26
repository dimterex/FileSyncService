using Newtonsoft.Json;

namespace SdkProject.Api.Attach
{
    public class UploadRequest : IMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}