using Newtonsoft.Json;
using System.Collections.Generic;

namespace Service.Api.Message
{
    public class BaseFileInfo
    {
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "file_path")]
        public List<string> FilePath { get; set; }

        public BaseFileInfo()
        {
            FileName = string.Empty;
            FilePath = new List<string>();
        }
    }
}
