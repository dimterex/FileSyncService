using Core.Process;

using VpnConnectionService._Interfaces_;

public class ProxyService : IProxyService
{
    #region Constants

    private const string PROXY_REGEDIT = "HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";

    private const string ENABLE_PARAMS = "/v ProxyEnable /t REG_DWORD /d 1 /f";
    private const string DISABLE_PARAMS = "/v ProxyEnable /t REG_DWORD /d 0 /f";

    #endregion

    #region Fields

    private readonly IProcessService _processService;

    #endregion

    #region Constructors

    public ProxyService(IProcessService processService)
    {
        _processService = processService;
    }

    #endregion

    #region Methods

    public void Enable()
    {
        _processService.StartProcess("reg.exe", $"add \"{PROXY_REGEDIT}\" {ENABLE_PARAMS}");
    }

    public void Disable()
    {
        _processService.StartProcess("reg.exe", $"add \"{PROXY_REGEDIT}\" {DISABLE_PARAMS}");
    }

    #endregion
}
