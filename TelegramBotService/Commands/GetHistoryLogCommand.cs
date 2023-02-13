namespace TelegramBotService.Commands
{
    using System;
    using System.Linq;

    using _Interfaces_;

    using Core.Publisher._Interfaces_;

    using ServicesApi;
    using ServicesApi.Common;
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
            IMessage response = _publisherService.CallAsync(QueueConstants.SYNC_APPLICATION_QUEUE, new GetHistoryRequest());

            IMessage responseResult = response;
            if (responseResult is not StatusResponse statusResponse)
                return;
            
            switch (statusResponse.Status)
            {
                case Status.Ok:
                    // _publisherService.CallAsync(QueueConstants.TELEGRAM_QUEUE, new SendTelegramMessageRequest()
                    // {
                    //     Message = statusResponse.Message,
                    // });
                    break;
                case Status.Error:
                    _publisherService.CallAsync(QueueConstants.TELEGRAM_QUEUE, new SendTelegramMessageRequest()
                    {
                        Message = statusResponse.Message,
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
