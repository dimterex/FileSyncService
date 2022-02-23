using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using ServicesApi.Common;
using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi
{
    public class RabbitMqPacketSerializer
    {
        #region Fields

        private readonly Dictionary<Type, string> _messageEnc;
        private readonly Dictionary<Type, string> _messageQueue;

        private readonly Dictionary<string, Type> _messageDec;

        #endregion Fields

        #region Constructors

        public RabbitMqPacketSerializer()
        {
            _messageDec = new Dictionary<string, Type>();
            _messageEnc = new Dictionary<Type, string>();
            _messageQueue = new Dictionary<Type, string>();
            Initialize(typeof(RabbitMqPacketSerializer).Assembly);
        }

        #endregion Constructors

        #region Methods

        public RabbitMqMessageContainer Serialize(IMessage message)
        {
            return new RabbitMqMessageContainer
            {
                Identifier = _messageEnc[message.GetType()],
                Value = message,
                Queue = _messageQueue[message.GetType()],
            };
        }

        public IMessage Deserialize(RabbitMqMessageContainer container)
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
                var attr = type.GetCustomAttribute<RabbitMqApiMessageAttribute>();
                if (attr == null)
                    continue;

                var id = $"{attr.Id}";
                _messageDec[id] = type;
                _messageEnc[type] = id;
                _messageQueue[type] = attr.Queue;
            }
        }

        #endregion Methods
    }
}