using System;
using Core.Daemon;
using Core.Logger;
using Microsoft.Extensions.DependencyInjection;
using PublicLogger._Interfaces_;

namespace PublicLogger
{
    class Program
    {
        public const string RABBIT_HOST = "RABBIT_HOST";
        
        static void Main(string[] args)
        {
            var daemon = new Daemon();
            
            daemon.Run(() =>
            {
                var services = new ServiceCollection();
                services.AddSingleton<ILoggerService, LoggerService>();
                services.AddSingleton<IRootService, RootService>();
                
                
                var serviceProvider = services.BuildServiceProvider();
                var rootService = serviceProvider.GetService<IRootService>();
                serviceProvider.GetService<CoreModule>();
                serviceProvider.GetService<FilesModule>();
                serviceProvider.GetService<ConfigurationModule>();
                rootService.Start();
            });
        }
    }
}