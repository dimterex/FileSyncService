using SdkProject;

namespace TransportProject
{
    public interface IClient
    {
        string ID { get; }
        bool IsConnected { get; set; }
        
        void SendMessage(IMessage result);
        
        void Close();
    }
}