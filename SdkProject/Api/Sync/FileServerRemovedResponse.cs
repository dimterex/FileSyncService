using Newtonsoft.Json;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Sync
{
    [SdkApiMessage("FileServerRemovedResponse")]
    public class FileServerRemovedResponse : ISdkMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string[] FileName { get; set; }
    }
}