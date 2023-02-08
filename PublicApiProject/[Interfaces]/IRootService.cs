namespace PublicProject._Interfaces_
{
    using Core.Publisher._Interfaces_;

    public interface IRootService
    {
        IPublisherService PublisherService { get; }
        void Start(int httpPort, int httpsPort);
    }
}
