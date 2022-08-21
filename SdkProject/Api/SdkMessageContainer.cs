using Newtonsoft.Json;

namespace SdkProject.Api
{
    public class SdkMessageContainer
    {
        #region Methods

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        #endregion Methods

        #region Properties

        [JsonProperty(PropertyName = "Type")] public string Identifier { get; set; }

        [JsonProperty(PropertyName = "Value")] public object Value { get; set; }

        #endregion Properties
    }
}