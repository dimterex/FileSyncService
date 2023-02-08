namespace PublicApiProject
{
    using System;

    using Common.DatabaseProject;
    using Common.DatabaseProject._Interfaces_;

    using Core.Daemon;

    using FileSystemProject;

    using Microsoft.Extensions.DependencyInjection;

    using PublicProject;
    using PublicProject._Interfaces_;
    using PublicProject._Interfaces_.Factories;
    using PublicProject.Actions;
    using PublicProject.Database.Actions.States;
    using PublicProject.Database.Actions.Users;
    using PublicProject.Factories;
    using PublicProject.Logic;
    using PublicProject.Logic.Comparing;
    using PublicProject.Modules;

    internal class Program
    {
        public const string RABBIT_HOST = "RABBIT_HOST";
        private const string DB_PATH = "DB_PATH";

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
                    services.AddSingleton<ISyncStateFilesResponseFactory, SyncStateFilesResponseFactory>();
                    services.AddSingleton<ISyncStateFilesResponseTaskFactory, SyncStateFilesResponseTaskFactory>();
                    services.AddSingleton<IConnectionRequestTaskFactory, ConnectionRequestTaskFactory>();
                    services.AddSingleton<ISyncStateFilesResponseService, SyncStateFilesResponseService>();
                    services.AddSingleton<IRootService, RootService>();
                    services.AddSingleton<IHistoryService, HistoryService>();

                    string dbPath = Environment.GetEnvironmentVariable(DB_PATH);
                    services.AddSingleton<IDataBaseFactory>(new DataBaseFactory(dbPath));

                    services.AddSingleton<IApiController, ApiController>();
                    services.AddSingleton<AvailableFoldersForUserRequestExecutor>();
                    services.AddSingleton<SyncStatesRequestExecutor>();
                    services.AddSingleton<AddNewStatesExecutor>();
                    services.AddSingleton<RemoveSyncStatesExecutor>();
                    services.AddSingleton<RemoveSyncStatesByAvailableFolderExecutor>();
                    services.AddSingleton<AddNewStateExecutor>();
                    services.AddSingleton<AddNewUserInfoExecutor>();
                    services.AddSingleton<RemoveUserInfoExecutor>();

                    services.AddSingleton<WsService>();

                    // Rest
                    services.AddSingleton<CoreModule>();
                    services.AddSingleton<FilesModule>();

                    // RabbitMQ
                    services.AddSingleton<ClearEmptyDirectoriesAction>();
                    services.AddSingleton<CreateUserAction>();
                    services.AddSingleton<GetHistoryAction>();
                    services.AddSingleton<UpdateSyncStateAction>();

                    // Compare services
                    services.AddSingleton<IFilesComparing, ClientAddFiles>();
                    services.AddSingleton<IFilesComparing, ClientRemoveFiles>();
                    services.AddSingleton<IFilesComparing, ClientServerExistFiles>();
                    services.AddSingleton<IFilesComparing, ClientUpdateFiles>();
                    services.AddSingleton<IFilesComparing, ServerAddFiles>();
                    services.AddSingleton<IFilesComparing, ServerRemoveFiles>();

                    ServiceProvider serviceProvider = services.BuildServiceProvider();
                    var rootService = serviceProvider.GetService<IRootService>();
                    serviceProvider.GetService<CoreModule>();
                    serviceProvider.GetService<FilesModule>();

                    rootService.Start(HTTP_PORT, HTTPS_PORT);
                });
        }
    }
}
