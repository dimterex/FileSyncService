using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotService._Interfaces_
{
    public interface IReceivedAction
    {
        void Execute(ITelegramBotClient botClient, Message updateMessage);
        void Configure(string message, string comment, ITelegramCommand callback);
    }
}