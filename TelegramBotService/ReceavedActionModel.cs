namespace TelegramBotService
{
    using System;

    using Telegram.Bot;
    using Telegram.Bot.Types;

    public struct ReceavedActionModel
    {
        public ReceavedActionModel(string comment, Action<ITelegramBotClient, Message> action)
        {
            Comment = comment;
            Action = action;
        }

        public string Comment { get; }

        public Action<ITelegramBotClient, Message> Action { get; }
    }
}
