using System;

namespace ServicesApi.Common._Interfaces_
{
    public interface IMessageHandlerWithResponse <T> where T: IMessage
    {
        void Handler(T message, Action<IMessage> responseAction);
    }
}