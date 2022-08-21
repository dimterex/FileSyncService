using ServicesApi.Common._Interfaces_;

namespace Core.Publisher._Interfaces_
{
    public interface IPublisherService
    {
        void SendMessage(IMessage message);
    }
}