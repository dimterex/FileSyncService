﻿using Newtonsoft.Json;

namespace SdkProject.Api.Attach
{
    public class DownloadRequest : IMessage
    {
        [JsonProperty(PropertyName = "file_id")]
        public string FilePath { get; set; }
        
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}