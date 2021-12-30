using Newtonsoft.Json;

namespace SdkProject.Api.Sync
{
    [ApiMessage("FileUpdatedResponse")]
    public class FileUpdatedResponse : IMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string[] FileName { get; set; }
        
        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        public FileUpdatedResponse()
        {
            Size = 0;
        }
    }
}