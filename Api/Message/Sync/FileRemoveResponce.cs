﻿using Newtonsoft.Json;
using Service.Api.Interfaces;

namespace Service.Api.Message.Sync
{
    using Service.Attribute.Api;

    [ApiMessage("FileRemoveResponse")]
    public class FileRemoveResponce : IMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }

    }
}
