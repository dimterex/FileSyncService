using SdkProject;
using WebSocketSharp.Server;

namespace TransportProject
{
    public interface IClient
    {
        string ID { get; }

        void SendResponse(HttpRequestEventArgs e, IMessage response);
        
        void Close();
    }
}