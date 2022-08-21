using Newtonsoft.Json;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Sync
{
    [SdkApiMessage("FileServerRemovedResponse")]
    public class FileDataBaseAddResponse : ISdkMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string[] FileName { get; set; }
    }
}