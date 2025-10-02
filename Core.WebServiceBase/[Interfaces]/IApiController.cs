namespace Core.WebServiceBase._Interfaces_;

using Models;

using SdkProject._Interfaces_;

public interface IApiController
{
    void SetErrorResponse(HttpRequestEventModel e);
    void RegisterRequest(string resource, BaseApiModule module);
    void HandleRequest(string resource, HttpRequestEventModel e);
    void SendResponse(HttpRequestEventModel e, ISdkMessage response);
}
