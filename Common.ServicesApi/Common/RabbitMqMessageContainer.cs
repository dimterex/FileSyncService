namespace ServicesApi.Common
{
    using Newtonsoft.Json;

    public class RabbitMqMessageContainer
    {
        #region Methods

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        #endregion Methods

        #region Properties

        [JsonProperty(PropertyName = "type")]
        public string Identifier { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        #endregion Properties
    }
}
