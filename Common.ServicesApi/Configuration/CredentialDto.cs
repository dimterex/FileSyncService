namespace ServicesApi.Configuration
{
    using Newtonsoft.Json;

    public class CredentialDto
    {
        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "domain")]
        public string Domain { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }
}
