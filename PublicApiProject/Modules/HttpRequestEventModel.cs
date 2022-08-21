using WebSocketSharp.Net;

namespace PublicProject.Modules
{
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
}