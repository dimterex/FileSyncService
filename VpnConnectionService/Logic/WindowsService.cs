using System;
using System.IO;
using System.Reflection;

using Core.Process;

using VpnConnectionService._Interfaces_;

public class WindowsService : IWindowsService
{
    private readonly IProcessService _processService;
    private readonly ISettingsService _settingsService;
    private readonly string _service_path;

    const string SERVICE_NAME = ".NET dimterex network helper";

    public WindowsService(IProcessService processService, ISettingsService settingsService)
    {
        _processService = processService;
        _settingsService = settingsService;

        var service_path = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Uri.LocalPath;

        string extension = Path.GetExtension(service_path); 
        service_path = service_path.Replace(extension,".exe");
        _service_path = Uri.UnescapeDataString(service_path);
    }
    
    public void InstallService()
    {
        var createArgs = $"create \"{SERVICE_NAME}\" binpath=\"{_service_path}\" start=delayed-auto ";
        if (_settingsService.Settings.RunAsUsername)
        {
            createArgs += $"obj=\"{_settingsService.Settings.Username}\" password=\"{_settingsService.Settings.Password}\"";
        }
        _processService.StartProcess("sc", createArgs);
        _processService.StartProcess("sc", $"failure \"{SERVICE_NAME}\" reset=0 actions=restart/0/restart/0/restart/0");
        _processService.StartProcess("sc", $"start \"{SERVICE_NAME}\"");
    }
    
    public void UninstallService()
    {
        _processService.StartProcess("sc", $"stop \"{SERVICE_NAME}\"");
        _processService.StartProcess("sc", $"delete \"{SERVICE_NAME}\"");
    }
}
