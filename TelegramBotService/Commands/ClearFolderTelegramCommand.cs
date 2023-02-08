namespace TelegramBotService.Commands
{
    using System.Collections.Generic;

    using _Interfaces_;

    using Core.Publisher._Interfaces_;

    using Database.Actions;

    using ServicesApi;
    using ServicesApi.Common._Interfaces_;
    using ServicesApi.Telegram;

    public class ClearFolderTelegramCommand : ITelegramCommand
    {
        private readonly AvailableFoldersRequestExecutor _availableFoldersRequestExecutor;
        private readonly IPublisherService _publisherService;

        public ClearFolderTelegramCommand(IPublisherService publisherService, AvailableFoldersRequestExecutor availableFoldersRequestExecutor)
        {
            _publisherService = publisherService;
            _availableFoldersRequestExecutor = availableFoldersRequestExecutor;
        }

        public async void Handle()
        {
            IList<string> filePaths = _availableFoldersRequestExecutor.Handler();

            IMessage response = _publisherService.CallAsync(
                QueueConstants.TELEGRAM_QUEUE,
                new SendTelegramMessageRequest
                {
                    Message = "test response"
                });

            IMessage responseResult = response;
            var tt = false;

            // var response = _publisherService.CallAsync(QueueConstants.FILE_STORAGE_QUEUE, new ClearEmptyDirectoriesRequest
            // {
            //     FilePaths = filePaths.ToArray()
            // });
        }
    }
}
