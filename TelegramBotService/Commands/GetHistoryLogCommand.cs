namespace TelegramBotService.Commands
{
    using System.Linq;

    using _Interfaces_;

    using Core.Publisher._Interfaces_;

    using ServicesApi;
    using ServicesApi.Common._Interfaces_;
    using ServicesApi.History;
    using ServicesApi.Telegram;

    public class GetHistoryLogCommand : ITelegramCommand
    {
        private readonly IPublisherService _publisherService;

        public GetHistoryLogCommand(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        public void Handle()
        {
            IMessage response = _publisherService.CallAsync(QueueConstants.FILE_STORAGE_QUEUE, new GetHistoryRequest());

            IMessage responseResult = response;

            if (responseResult is GetHistoryResponse getHistoryResponse)
                _publisherService.CallAsync(
                    QueueConstants.TELEGRAM_QUEUE,
                    new SendTelegramMessageRequest
                    {
                        Message = getHistoryResponse.Items.FirstOrDefault().File
                    });
        }
    }
}
