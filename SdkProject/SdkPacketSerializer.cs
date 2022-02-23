using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;
using SdkProject.Api;

namespace SdkProject
{
    public class SdkPacketSerializer
    {
        #region Fields

        private readonly Dictionary<Type, string> _messageEnc;
        private readonly Dictionary<string, Type> _messageDec;

        #endregion Fields

        #region Constructors

        public SdkPacketSerializer()
        {
            _messageDec = new Dictionary<string, Type>();
            _messageEnc = new Dictionary<Type, string>();
            Initialize(typeof(SdkPacketSerializer).Assembly);
        }

        #endregion Constructors

        #region Methods

        public SdkMessageContainer Serialize(ISdkMessage sdkMessage)
        {
            return new SdkMessageContainer
            {
                Identifier = _messageEnc[sdkMessage.GetType()],
                Value = sdkMessage,
            };
        }

        public ISdkMessage Deserialize(SdkMessageContainer container)
        {
            if (!_messageDec.TryGetValue(container.Identifier, out var type))
                return null;

            var message = (ISdkMessage)((JObject)container.Value).ToObject(type);

            return message;
        }
        
        private void Initialize(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
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