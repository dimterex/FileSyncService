namespace VpnConnectionService
{
    using System;

    using _Interfaces_;

    using Core.Daemon;
    using Core.Process;
    using Core.Publisher;
    using Core.Publisher._Interfaces_;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    class Program
    {
        public const string RABBIT_HOST = "RABBIT_HOST";
        
        static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices(
                    (hostBuilderContext, services) =>
                    {
                        string host = Environment.GetEnvironmentVariable(RABBIT_HOST);
                        services.AddSingleton<IPublisherService>(new RpcPublisherService(host));
                        
                        services.AddSingleton<INetworkService, NetworkService>();
                        services.AddSingleton<IProcessService, ProcessService>();
                        services.AddSingleton<IProxyService, ProxyService>();
                        services.AddSingleton<ISettingsService, SettingsService>();
                        services.AddSingleton<IVpnService, VpnService>();
                        services.AddSingleton<IWindowsService, WindowsService>();
                        services.AddSingleton<IRootService, RootService>();
            
                        services.AddHostedService<RootService>();
           
                    })
                .Build();
            
            if (args.Length > 0)
            {
                var windowsService = host.Services.GetService<IWindowsService>();
                switch (args[0])
                {
                    case "/i": 
                        windowsService.InstallService();
                        break;
                    case "/u":
                        windowsService.UninstallService();
                        break;
                }
            }
            else
            {
                host.Run();
            }
        }
    }
}
