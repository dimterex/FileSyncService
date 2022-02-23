﻿using Newtonsoft.Json;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Sync
{
    [SdkApiMessage("FileUpdatedResponse")]
    public class FileUpdatedResponse : ISdkMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string[] FileName { get; set; }
        
        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        public FileUpdatedResponse()
        {
            Size = 0;
        }
    }
}