namespace SdkProject.Api.Sync.Common
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class FolderItem
    {
        #region Constructors

        public FolderItem()
        {
            Files = new List<FileItem>();
        }

        #endregion

        #region Properties

        [JsonProperty(PropertyName = "files")]
        public List<FileItem> Files { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string[] DictionaryPath { get; set; }

        #endregion
    }
}
