using System;

namespace ServicesApi.Common._Attribute_
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RabbitMqApiMessageAttribute : Attribute
    {
        #region Constructors

        public RabbitMqApiMessageAttribute(string queue, string id)
        {
            Id = id;
            Queue = queue;
        }

        #endregion Constructors

        #region Properties

        public string Id { get; }

        public string Queue { get; }

        #endregion Properties
    }
}