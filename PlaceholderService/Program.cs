namespace PlaceholderService
{
    using Core.Daemon;
    using Core.WebServiceBase._Interfaces_;
    using Core.WebServiceBase.Services;

    using FileSystemProject;

    using Microsoft.Extensions.DependencyInjection;

    using PlaceholderService._Interfaces_;
    using PlaceholderService.Logic;
    using PlaceholderService.Modules;

    internal class Program
    {
        private const int HTTP_PORT = 1234;
        private const int HTTPS_PORT = 1235;

        private static void Main(string[] args)
        {
            var daemon = new Daemon();

            daemon.Run(
                () =>
                {
                    var services = new ServiceCollection();
                    services.AddSingleton<IConnectionStateManager, ConnectionStateManager>();
                    services.AddSingleton<IFileInfoModelFactory, FileInfoModelFactory>();
                    services.AddSingleton<IFileSystemService, FileSystemService>();
                    services.AddSingleton<IFileManager, FileManager>();
                    services.AddSingleton<ISyncStateFilesResponseService, SyncStateFilesResponseService>();


                    services.AddSingleton<IApiController, ApiController>();

                    services.AddSingleton<WsService>();

                    // Rest
                    services.AddSingleton<CoreModule>();
                    services.AddSingleton<FilesModule>();

                    ServiceProvider serviceProvider = services.BuildServiceProvider();
                    var wsService = serviceProvider.GetService<WsService>();
                    serviceProvider.GetService<CoreModule>();
                    serviceProvider.GetService<FilesModule>();

                    wsService.Start(HTTP_PORT, HTTPS_PORT);
                });
        }
    }
}
