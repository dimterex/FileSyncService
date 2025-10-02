namespace Core.WebServiceBase.Models;

using WebSocketSharp.Net;

public class HttpRequestEventModel
{
    public HttpRequestEventModel(HttpListenerRequest request, HttpListenerResponse response)
    {
        Request = request;
        Response = response;
    }

    public HttpListenerRequest Request { get; }
    public HttpListenerResponse Response { get; }
}
