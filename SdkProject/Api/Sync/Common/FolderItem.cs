using System.Collections.Generic;
using Newtonsoft.Json;

namespace SdkProject.Api.Sync.Common
{
    public class FolderItem
    {
        public FolderItem()
        {
            Files = new List<FileItem>();
            DictionaryPath = string.Empty;
        }

        [JsonProperty(PropertyName = "files")] public List<FileItem> Files { get; set; }

        [JsonProperty(PropertyName = "dictionary")]
        public string DictionaryPath { get; set; }
    }
}