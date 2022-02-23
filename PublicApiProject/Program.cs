using System;
using Core.Customer;
using Core.Publisher;
using FileSystemProject;
using PublicProject;
using PublicProject._Interfaces_;
using PublicProject.Actions;
using PublicProject.Modules;
using ServicesApi;

namespace PublicApiProject
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var publisherController = new PublisherController("dimterex.duckdns.org", "user", "1234");

            IConnectionStateManager connectionStateManager = new ConnectionStateManager();
            var fileservice = new FileSystemService();
            var apiController = new ApiController();

            var fileManager = new FileManager(fileservice);
            var syncModule = new CoreModule(fileManager, connectionStateManager, publisherController);
            syncModule.Initialize(apiController);
            
            var attachModule = new FilesModule(connectionStateManager, publisherController);
            attachModule.Initialize(apiController);

            var configModule = new ConfigurationModule(publisherController);
            configModule.Initialize(apiController);
            
            var customerController = new CustomerController("dimterex.duckdns.org", "user", "1234", QueueConstants.FILE_STORAGE_QUEUE);
           
            // States
            customerController.Configure(new ClearEmptyDirectoriesAction(publisherController, fileManager));
                
            var port = 1234;
            var ports = 1235;
            var ws = new WsService(apiController,  port, ports);
            ws.Start();
            
            // var services = new ServiceCollection();
            // services.AddSingleton<IUserService, UserSerice>();
            // services.AddSingleton<UserApplication>();
            // var serviceProvider = services.BuildServiceProvider();
            // var userAppService = serviceProvider.GetService<UserApplication>();

            Console.ReadLine();
            
            ws.Stop();
        }
    }
}