using Newtonsoft.Json;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Sync
{
    [SdkApiMessage("FileUploadRequest")]
    public class FileUploadRequest : ISdkMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string[] FileName { get; set; }
    }
}