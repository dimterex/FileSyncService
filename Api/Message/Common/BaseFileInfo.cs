namespace Service.Api.Message.Common
{
    using Newtonsoft.Json;

    using Service.Api.Interfaces;

    using System.Collections.Generic;

    public class BaseFileInfo : IMessage
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
