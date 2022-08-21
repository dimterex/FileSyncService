using System.Linq;
using Core.Publisher;
using ServicesApi.FileStorage;
using TelegramBotService._Interfaces_;
using TelegramBotService.Database.Actions;

namespace TelegramBotService.Commands
{
    public class ClearFolderTelegramCommand : ITelegramCommand
    {
        private readonly AvailableFoldersRequestExecutor _availableFoldersRequestExecutor;
        private readonly PublisherService _publisherService;

        public ClearFolderTelegramCommand(PublisherService publisherService,
            AvailableFoldersRequestExecutor availableFoldersRequestExecutor)
        {
            _publisherService = publisherService;
            _availableFoldersRequestExecutor = availableFoldersRequestExecutor;
        }

        public void Handle()
        {
            var filePaths = _availableFoldersRequestExecutor.Handler();

            _publisherService.SendMessage(new ClearEmptyDirectories
            {
                FilePaths = filePaths.ToArray()
            });
        }
    }
}