namespace ServicesApi
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Common;
    using Common._Attribute_;
    using Common._Interfaces_;

    using Newtonsoft.Json.Linq;

    public class RabbitMqPacketSerializer
    {
        #region Constructors

        public RabbitMqPacketSerializer()
        {
            _messageDec = new Dictionary<string, Type>();
            _messageEnc = new Dictionary<Type, string>();
            Initialize(typeof(RabbitMqPacketSerializer).Assembly);
        }

        #endregion Constructors

        #region Fields

        private readonly Dictionary<Type, string> _messageEnc;

        private readonly Dictionary<string, Type> _messageDec;

        #endregion Fields

        #region Methods

        public RabbitMqMessageContainer Serialize(IMessage message)
        {
            return new RabbitMqMessageContainer
            {
                Identifier = _messageEnc[message.GetType()],
                Value = message
            };
        }

        public IMessage Deserialize(RabbitMqMessageContainer container)
        {
            if (!_messageDec.TryGetValue(container.Identifier, out Type type))
                return null;

            var message = (IMessage)((JObject)container.Value).ToObject(type);

            return message;
        }

        private void Initialize(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                var attr = type.GetCustomAttribute<RabbitMqApiMessageAttribute>();
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
