using Newtonsoft.Json;

namespace SdkProject.Api.Sync
{
    [ApiMessage("FileRemoveResponse")]
    public class FileRemoveResponce : IMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }

    }
}
