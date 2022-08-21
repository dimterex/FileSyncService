namespace ServicesApi.Common._Interfaces_
{
    public interface IMessageHandler<T> where T : IMessage
    {
        void Handler(T message);
    }
}