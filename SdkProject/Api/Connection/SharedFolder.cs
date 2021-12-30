using System.Collections.Generic;
using Newtonsoft.Json;

namespace SdkProject.Api.Connection
{
    public class SharedFolder
    {
        [JsonProperty(PropertyName = "path")]
        public List<string> Files { get; set; }

        public SharedFolder()
        {
            Files = new List<string>();
        }
    }
}