using Newtonsoft.Json;

namespace Service.Api.Message
{
    public class FileAddResponce : BaseFileInfo
    {
        [JsonProperty(PropertyName = "stream")]
        public string Stream { get; set; }

        public FileAddResponce()
        {
            Stream = string.Empty;
        }
    }
}
