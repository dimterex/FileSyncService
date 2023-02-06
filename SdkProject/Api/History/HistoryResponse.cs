using System.Collections.Generic;
using Newtonsoft.Json;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;

namespace SdkProject.Api.History
{
    [SdkApiMessage("history_response")]
    public class HistoryResponse : ISdkMessage
    {
        [JsonProperty(PropertyName = "items")]
        public List<HistoryModel> Items { get; set; }

        public HistoryResponse()
        {
            Items = new List<HistoryModel>();
        }
    }
}