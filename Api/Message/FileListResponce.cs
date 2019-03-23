using Newtonsoft.Json;
using System.Collections.Generic;

namespace Service.Api.Message
{
    public class FileListResponce
    {
        [JsonProperty(PropertyName = "files")]
        public List<BaseFileInfo> Files { get; set; }

        public FileListResponce()
        {
            Files = new List<BaseFileInfo>();
        }
    }
}
