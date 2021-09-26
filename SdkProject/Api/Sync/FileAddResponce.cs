using Newtonsoft.Json;

namespace SdkProject.Api.Sync
{
    [ApiMessage("FileAddResponse")]
    public class FileAddResponce : IMessage
    {

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }
        
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }


        public FileAddResponce()
        {
            Size = 0;
        }
    }
}
