namespace ServicesApi.Common._Attribute_
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class RabbitMqApiMessageAttribute : Attribute
    {
        #region Constructors

        public RabbitMqApiMessageAttribute(string id)
        {
            Id = id;
        }

        #endregion Constructors

        #region Properties

        public string Id { get; }

        #endregion Properties
    }
}
