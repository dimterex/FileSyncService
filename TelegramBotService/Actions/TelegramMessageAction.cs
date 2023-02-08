namespace TelegramBotService.Actions
{
    using _Interfaces_;

    using ServicesApi.Common;
    using ServicesApi.Common._Interfaces_;
    using ServicesApi.Telegram;

    public class TelegramMessageAction : IMessageHandler<SendTelegramMessageRequest>
    {
        private readonly ITelegramService _telegramService;

        public TelegramMessageAction(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        public IMessage Handler(SendTelegramMessageRequest messageRequest)
        {
            _telegramService.SendTextMessageAsync(messageRequest.Message);
            return new StatusResponse
            {
                Message = "Sent success",
                Status = Status.Ok
            };
        }
    }
}
