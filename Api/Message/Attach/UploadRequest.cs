﻿using Newtonsoft.Json;
using Service.Api.Interfaces;

namespace Service.Api.Message.Attach
{
    public class UploadRequest : IMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}