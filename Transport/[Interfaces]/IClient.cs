using Service.Api.Interfaces;

namespace Service.Transport
{
    public interface IClient
    {
        string ID { get; }
        bool IsConnected { get; set; }
        
        void SendMessage(IMessage result);
        
        void Close();
    }
}
