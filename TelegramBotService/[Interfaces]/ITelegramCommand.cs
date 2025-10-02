namespace TelegramBotService._Interfaces_
{
    using Telegram.Bot;

    public interface ITelegramCommand
    {
        void Handle(ITelegramBotClient botClient, long chatId, int replyToMessageId, string message);
    }
}
