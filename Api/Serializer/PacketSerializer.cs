namespace Service.Api.Serializer
{
    using Newtonsoft.Json.Linq;

    using Service.Api.Interfaces;
    using Service.Api.Message;
    using Service.Attribute.Api;

    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class PacketSerializer
    {
        #region Fields

        private readonly Dictionary<Type, string> _messageEnc;

        private readonly Dictionary<string, Type> _messageDec;

        #endregion Fields

        #region Constructors

        public PacketSerializer()
        {
            _messageDec = new Dictionary<string, Type>();
            _messageEnc = new Dictionary<Type, string>();
            Initialize(typeof(PacketSerializer).Assembly);
        }

        #endregion Constructors

        #region Methods

        public MessageContainer Serialize(IMessage message)
        {
            return new MessageContainer
            {
                Identifier = _messageEnc[message.GetType()],
                Value = message,
            };
        }

        public IMessage Deserialize(MessageContainer container)
        {
            if (!_messageDec.TryGetValue(container.Identifier, out var type))
                return null;

            var message = (IMessage)((JObject)container.Value).ToObject(type);

            return message;
        }

        private void Initialize(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var attr = type.GetCustomAttribute<ApiMessageAttribute>();
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
