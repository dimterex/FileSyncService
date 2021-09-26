using System;

namespace SdkProject
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ApiMessageAttribute : Attribute
    {
        #region Properties

        public string Id { get; }

        #endregion Properties

        #region Constructors

        public ApiMessageAttribute(string id)
        {
            Id = id;
        }

        #endregion Constructors
    }
}