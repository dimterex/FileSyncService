using System;
using Core.Customer;
using Core.Daemon;
using Core.Publisher;
using NLog;
using ServicesApi;

namespace DataBaseWriterService
{
    static class Program
    {
        private const string RABBIT_HOST = "RABBIT_HOST";
        private const string DB_PATH = "DB_PATH";
        
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(() => $"Starting...");
            
            var host = Environment.GetEnvironmentVariable(RABBIT_HOST);
            var dbPath = Environment.GetEnvironmentVariable(DB_PATH);

            var daemon = new Daemon();
            logger.Info(() => $"Host: {host}");
            logger.Info(() => $"Database Path: {dbPath}");
            
            daemon.Run(() =>
            {
                var customerController = new CustomerController(host, QueueConstants.DATABASE_QUEUE);
                var dataBaseFactory = new DataBaseFactory(dbPath);
            
                // States
                customerController.Configure(new AddNewStateAction(dataBaseFactory));
                customerController.Configure(new AddNewStatesAction(dataBaseFactory));
                customerController.Configure(new RemoveSyncStatesAction(dataBaseFactory));
                customerController.Configure(new RemoveSyncStatesByAvailableFolderAction(dataBaseFactory));
            
                customerController.ConfigureWithResponse(new SyncStatesRequestAction(dataBaseFactory));
            
                // Users
                customerController.Configure(new AddNewUserInfoAction(dataBaseFactory));
                customerController.ConfigureWithResponse(new AvailableFoldersForUserRequestAction(dataBaseFactory));
                customerController.ConfigureWithResponse(new AvailableFoldersRequestAction(dataBaseFactory));
                customerController.Configure(new RemoveUserInfoAction(dataBaseFactory));
                
                logger.Info(() => $"Started");
            });
            
        }
    }
}