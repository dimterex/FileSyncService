using Newtonsoft.Json;

namespace ServicesApi.Common
{
    public class RabbitMqMessageContainer
    {
        #region Methods

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        #endregion Methods

        #region Properties

        [JsonProperty(PropertyName = "type")] public string Identifier { get; set; }

        [JsonProperty(PropertyName = "value")] public object Value { get; set; }

        [JsonIgnore] public string Queue { get; set; }

        #endregion Properties
    }
}