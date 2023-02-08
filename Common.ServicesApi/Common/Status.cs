// ---------------------------------------------------------------------------------------------------------------------------------------------------
// Copyright ElcomPlus LLC. All rights reserved.
// Author: 
// ---------------------------------------------------------------------------------------------------------------------------------------------------

namespace ServicesApi.Common
{
    using System.Runtime.Serialization;

    public enum Status
    {
        [EnumMember(Value = "ok")]
        Ok,

        [EnumMember(Value = "error")]
        Error,
    }
}
