using System;
using Newtonsoft.Json;

namespace ExecutableProject
{
    public class Settings
    {

        [JsonProperty(PropertyName = "http_port")]
        public int HttpPort { get; set; }
        
        [JsonProperty(PropertyName = "https_port")]
        public int HttpsPort { get; set; }
        
        [JsonProperty(PropertyName = "ip")]
        public string IpAddress { get; set; }
        
        [JsonProperty(PropertyName = "telegram_id")]
        public int Telegram_id { get; set; }
        
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}