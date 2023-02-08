namespace Core.Publisher._Interfaces_
{
    using ServicesApi.Common._Interfaces_;

    public interface IPublisherService
    {
        IMessage CallAsync(string queue, IMessage message);
    }
}
