using Newtonsoft.Json;

namespace SdkProject.Api.Sync
{
    [ApiMessage("FileUploadRequest")]
    public class FileUploadRequest : IMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }
    }
}