using Newtonsoft.Json.Linq;

namespace Service.Json
{
    public class JMessage
    {
        public string Type { get; set; }
        public JToken Value { get; set; }

        public static JMessage FromValue<T>(T value)
        {
            return new JMessage { Type = typeof(T).Name, Value = JToken.FromObject(value) };
        }

        public static string Serialize(JMessage message)
        {
            return JToken.FromObject(message).ToString();
        }

        public static JMessage Deserialize(string data)
        {
            return JToken.Parse(data).ToObject<JMessage>();
        }
    }
}
