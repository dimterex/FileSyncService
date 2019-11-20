namespace Service.Api.Message
{
    using Newtonsoft.Json;

    public class MessageContainer
    {
        #region Properties

        [JsonProperty(PropertyName = "Type")]
        public string Identifier { get; set; }

        [JsonProperty(PropertyName = "Value")]
        public object Value { get; set; }

        #endregion Properties

        #region Methods

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);

        #endregion Methods
    }
}
