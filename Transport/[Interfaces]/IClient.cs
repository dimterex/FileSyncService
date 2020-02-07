using Service.Api.Message;
using System;

namespace Service.Transport
{
    public interface IClient
    {
        bool IsConnected { get; set; }

        void SendMessage(MessageContainer result);
    }
}
