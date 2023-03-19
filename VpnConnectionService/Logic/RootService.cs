using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using NLog;

using VpnConnectionService._Interfaces_;
using VpnConnectionService.Models;

using Timer = System.Timers.Timer;

public class RootService : BackgroundService, IRootService 
{
    #region Fields

    private readonly INetworkService _networkService;
    private readonly IProxyService _proxyService;
    private readonly SettingsModel _settingsModel;
    private readonly Timer _timer;
    private readonly IVpnService _vpnService;
    private readonly ILogger _logger;

    #endregion

    #region Constructors

    public RootService(INetworkService networkService, IProxyService proxyService, IVpnService vpnService, ISettingsService settingsService)
    {
        _logger = LogManager.GetLogger(nameof(RootService));
        _networkService = networkService;
        _proxyService = proxyService;
        _vpnService = vpnService;
        _settingsModel = settingsService.Settings;
        _networkService.NetworkChangeEvent += NetworkServiceNetworkChangeEvent;
        // using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        // {
        //     WindowsPrincipal principal = new WindowsPrincipal(identity);
        //     _logger.Debug(() => $"Admin rules: {principal.IsInRole(WindowsBuiltInRole.Administrator)}");
        // }
    }

    #endregion

    #region Methods

    private void NetworkServiceNetworkChangeEvent(object sender, string connectedSsid)
    {
        _vpnService.Stop();
        _proxyService.Disable();

        if (string.IsNullOrEmpty(connectedSsid))
            return;

        if (connectedSsid == _settingsModel.WorkSsid)
            _proxyService.Enable();
        else
            _vpnService.Start();
    }

    #endregion

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
        }

        _networkService.NetworkChangeEvent -= NetworkServiceNetworkChangeEvent;
        return Task.CompletedTask;
    }
}
