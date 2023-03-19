using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

using NLog;

using VpnConnectionService._Interfaces_;

public class NetworkService : INetworkService
{
    #region Constructors

    public NetworkService(IProcessService processService)
    {
        _processService = processService;
        _logger = LogManager.GetLogger(nameof(NetworkService));
        _lastNetworkSsid = string.Empty;

        _timer = new Timer();
        _timer.Elapsed += OnTimedEvent;
        _timer.Interval = 5000;
        _timer.Enabled = true;
    }

    #endregion

    #region Events

    public event EventHandler<string> NetworkChangeEvent;

    #endregion

    #region Fields

    private readonly IProcessService _processService;
    private readonly Timer _timer;
    private string _lastNetworkSsid;
    private readonly ILogger _logger;

    #endregion

    #region Methods

    private void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        string newSsid = GetConnectedInternet();
        if (newSsid == _lastNetworkSsid)
            return;

        _lastNetworkSsid = newSsid;
        _logger.Info(() => "New SSID: " + _lastNetworkSsid);
        NetworkChangeEvent?.Invoke(this, newSsid);
    }

    private string GetConnectedInternet()
    {
        IList<string> output = _processService.StartProcess("netsh.exe", "wlan show interfaces");

        string wifi_supported = output.FirstOrDefault(x => x.Contains("wlansvc"));
        _logger.Debug(() => $"wifi_supported: {wifi_supported}");

        if (!string.IsNullOrEmpty(wifi_supported))
            return wifi_supported;

        string? line = string.Join(string.Empty, output).Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).
            FirstOrDefault(l => l.Contains("SSID") && !l.Contains("BSSID"));

        if (string.IsNullOrEmpty(line))
            return string.Empty;

        string ssid = line.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].TrimStart();
        return ssid;
    }

    #endregion
}
