using System;
using System.Text;
using Core.Publisher;
using ServicesApi.Database.Users;
using ServicesApi.FileStorage;
using TelegramBotService._Interfaces_;

namespace TelegramBotService.Commands
{
    public class ClearFolderTelegramCommand : ITelegramCommand
    {
        private readonly PublisherController _publisherController;

        public ClearFolderTelegramCommand(PublisherController publisherController)
        {
            _publisherController = publisherController;
        }

        public void Handle()
        {
            var result = _publisherController.SendWithResponse(new AvailableFoldersRequest());

            if (result is AvailableFoldersResponse availableFoldersResponse)
            {
                _publisherController.Send(new ClearEmptyDirectories()
                {
                    FilePaths = availableFoldersResponse.FilePaths
                });
               
            }
        }
    }
}