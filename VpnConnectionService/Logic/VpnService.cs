using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Timers;

using NLog;

using VpnConnectionService._Interfaces_;
using VpnConnectionService.Models;

public class VpnService : IVpnService
{
    #region Constructors

    public VpnService(IProcessService processService, ISettingsService settingsService)
    {
        _processService = processService;
        _settings = settingsService.Settings;
        _timer = new Timer();
        _timer.Elapsed += OnTimedEvent;
        _timer.Interval = 5000;
        _logger = LogManager.GetLogger(nameof(VpnService));
    }

    #endregion

    #region Fields

    private readonly IProcessService _processService;
    private readonly SettingsModel _settings;
    private readonly Timer _timer;
    private readonly ILogger _logger;

    #endregion

    #region Methods

    private void OnTimedEvent(object? sender, ElapsedEventArgs e)
    {
        if (CheckConnection())
        {
            _logger.Info(() => "VPN connected...");
        }
        else
        {
            _logger.Info(() => "VPN connecting...");
            _processService.StartProcess("rasdial", $"\"{_settings.VpnName}\" \"{_settings.Username}\" \"{_settings.Password}\"");

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress? vpnIp = host.AddressList.FirstOrDefault(x => x.ToString().StartsWith("192.168.168"));

            if (vpnIp != null)
                ApplyRouters(vpnIp);
        }
    }

    public void Start()
    {
        _timer.Enabled = true;
    }

    public void Stop()
    {
        _timer.Enabled = false;
        _processService.StartProcess("rasdial", $"\"{_settings.VpnName}\" /disconnect");
    }

    private bool CheckConnection()
    {
        if (!NetworkInterface.GetIsNetworkAvailable())
            return false;

        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface networkInterface in interfaces)
        {
            if (networkInterface.Name == _settings.VpnName)
                return networkInterface.OperationalStatus == OperationalStatus.Up;
        }

        return false;
    }

    private void ApplyRouters(IPAddress vpnIp)
    {
        ApplyRoute($"add 192.168.0.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.0.30  mask 255.255.255.0 {vpnIp} metric 1");

        // jira
        ApplyRoute($"add 192.168.55.246  mask 255.255.255.0 {vpnIp} metric 1");

        // confluence
        ApplyRoute($"add 192.168.0.120  mask 255.255.255.0 {vpnIp} metric 1");

        ApplyRoute($"add 192.168.1.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.7.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.8.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.11.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.20.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.33.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.36.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.37.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.40.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.50.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.51.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.55.0  mask 255.255.255.0 {vpnIp} metric 1");
        ApplyRoute($"add 192.168.56.0  mask 255.255.255.0 {vpnIp} metric 1");
    }

    private void ApplyRoute(string route)
    {
        _processService.StartProcess("route", route);
    }

    #endregion
}
