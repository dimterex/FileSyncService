namespace VpnConnectionService._Interfaces_
{
    using System.Collections.Generic;

    public interface IProcessService
    {
        IList<string> StartProcess(string command, string args);
    }
}
