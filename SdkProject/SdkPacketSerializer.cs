namespace SdkProject
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using _Attribute_;

    using _Interfaces_;

    using Api;

    using Newtonsoft.Json.Linq;

    public class SdkPacketSerializer
    {
        #region Constructors

        public SdkPacketSerializer()
        {
            _messageDec = new Dictionary<string, Type>();
            _messageEnc = new Dictionary<Type, string>();
            Initialize(typeof(SdkPacketSerializer).Assembly);
        }

        #endregion Constructors

        #region Fields

        private readonly Dictionary<Type, string> _messageEnc;
        private readonly Dictionary<string, Type> _messageDec;

        #endregion Fields

        #region Methods

        public SdkMessageContainer Serialize(ISdkMessage sdkMessage)
        {
            return new SdkMessageContainer
            {
                Identifier = _messageEnc[sdkMessage.GetType()],
                Value = sdkMessage
            };
        }

        public ISdkMessage Deserialize(SdkMessageContainer container)
        {
            if (!_messageDec.TryGetValue(container.Identifier, out Type type))
                return null;

            var message = (ISdkMessage)((JObject)container.Value).ToObject(type);

            return message;
        }

        private void Initialize(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                var attr = type.GetCustomAttribute<SdkApiMessageAttribute>();
                if (attr == null)
                    continue;

                var id = $"{attr.Id}";
                _messageDec[id] = type;
                _messageEnc[type] = id;
            }
        }

        #endregion Methods
    }
}
