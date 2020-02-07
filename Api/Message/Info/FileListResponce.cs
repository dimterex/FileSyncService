namespace Service.Api.Message.Info
{
    using Newtonsoft.Json;

    using Service.Api.Interfaces;
    using Service.Api.Message.Common;
    using Service.Attribute.Api;

    using System.Collections.Generic;

    [ApiMessage("FileListResponce")]
    public class FileListResponce : IMessage
    {
        [JsonProperty(PropertyName = "files")]
        public List<BaseFileInfo> Files { get; set; }

        public FileListResponce()
        {
            Files = new List<BaseFileInfo>();
        }
    }
}
