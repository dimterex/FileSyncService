using Newtonsoft.Json;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Sync
{
    [SdkApiMessage("FileAddResponse")]
    public class FileAddResponse : ISdkMessage
    {

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }
        
        [JsonProperty(PropertyName = "file_name")]
        public string[] FileName { get; set; }


        public FileAddResponse()
        {
            Size = 0;
        }
    }
}
