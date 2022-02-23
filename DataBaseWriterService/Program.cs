using System;
using Core.Customer;
using Core.Publisher;
using DataBaseWriterService.Actions.States;
using DataBaseWriterService.Actions.Users;
using ServicesApi;

namespace DataBaseWriterService
{
    static class Program
    {
        static void Main(string[] args)
        {
            
            var customerController = new CustomerController("dimterex.duckdns.org", "user", "1234", QueueConstants.DATABASE_QUEUE);
            var dataBaseFactory = new DataBaseFactory();
            
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
            
            Console.ReadLine();
            
            customerController.Stop();
        }
    }
}