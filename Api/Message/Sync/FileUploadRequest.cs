using Newtonsoft.Json;
using Service.Api.Interfaces;
using Service.Attribute.Api;

namespace Service.Api.Message.Sync
{
    [ApiMessage("FileUploadRequest")]
    public class FileUploadRequest : IMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }
    }
}