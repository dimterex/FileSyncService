using Newtonsoft.Json;

namespace SdkProject.Api.Sync
{
    [ApiMessage("FileRemoveResponse")]
    public class FileRemoveResponse : IMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }

    }
}
