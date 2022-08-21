using Core.Publisher;
using Core.Publisher._Interfaces_;

namespace PublicProject._Interfaces_
{
    public interface IRootService
    {
        IPublisherService PublisherService { get; }
        void Start(int httpPort, int httpsPort);
    }
}