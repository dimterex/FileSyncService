using Newtonsoft.Json;

namespace SdkProject.Api.Confguration
{
    [ApiMessage("CreateUserRequest")]
    public class CreateUserRequest : IMessage
    {
        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }
        
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
        
        [JsonProperty(PropertyName = "available_folders")]
        public AvailableFolder[] AvailableFolders { get; set; }
    }
}