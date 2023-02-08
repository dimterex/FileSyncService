namespace TelegramBotService._Interfaces_
{
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public interface IReceivedAction
    {
        void Execute(ITelegramBotClient botClient, Message updateMessage);
        void Configure(string message, string comment, ITelegramCommand callback);
    }
}
