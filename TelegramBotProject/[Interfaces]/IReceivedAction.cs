using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotProject._Interfaces_
{
    public interface IReceivedAction
    {
        void Execute(ITelegramBotClient botClient, Message updateMessage);
        void Configure(string message, string comment, Action callback);
    }
}