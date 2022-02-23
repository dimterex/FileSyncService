using Newtonsoft.Json;

namespace ServicesApi.Common
{
    public class RabbitMqMessageContainer
    {
        #region Properties

        [JsonProperty(PropertyName = "Type")]
        public string Identifier { get; set; }

        [JsonProperty(PropertyName = "Value")]
        public object Value { get; set; }
        
        [JsonIgnore]
        public string Queue { get; set; }

        #endregion Properties

        #region Methods

        public string Serialize() => JsonConvert.SerializeObject(this, Formatting.Indented);

        #endregion Methods
    }
}