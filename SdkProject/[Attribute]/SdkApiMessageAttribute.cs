namespace SdkProject._Attribute_
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class SdkApiMessageAttribute : Attribute
    {
        #region Constructors

        public SdkApiMessageAttribute(string id)
        {
            Id = id;
        }

        #endregion Constructors

        #region Properties

        public string Id { get; }

        #endregion Properties
    }
}
