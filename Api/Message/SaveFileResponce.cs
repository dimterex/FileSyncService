using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Service.Api.Message
{
    public class SaveFileResponce
    {
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
