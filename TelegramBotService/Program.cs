using System;
using System.Text;
using Core.Customer;
using Core.Publisher;
using ServicesApi;
using TelegramBotService.Actions;
using TelegramBotService.Commands;

namespace TelegramBotService
{
    class Program
    {
        static void Main(string[] args)
        {
            var customerController = new CustomerController("dimterex.duckdns.org", "user", "1234", QueueConstants.TELEGRAM_QUEUE);
           
            // States
            customerController.Configure(new TelegramMessageAction());

            var publisherController = new PublisherController("dimterex.duckdns.org", "user", "1234");
            
            var telegramService = new TelegramService("token", 1234);

            telegramService.Configure("/clean_folders", "clean empty folders", new ClearFolderTelegramCommand(publisherController));
            
            Console.ReadKey(true);
        }
    }
}