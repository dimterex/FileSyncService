using ServicesApi.Common._Interfaces_;
using ServicesApi.Telegram;
using TelegramBotService._Interfaces_;

namespace TelegramBotService.Actions
{
    public class TelegramMessageAction : IMessageHandler<TelegramMessage>
    {
        private readonly ITelegramService _telegramService;

        public TelegramMessageAction(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        public void Handler(TelegramMessage message)
        {
            _telegramService.SendTextMessageAsync(message.Message);
        }
    }
}