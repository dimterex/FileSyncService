namespace VpnConnectionService._Interfaces_
{
    using System;

    public interface INetworkService
    {
        event EventHandler<string> NetworkChangeEvent;
    }
}
