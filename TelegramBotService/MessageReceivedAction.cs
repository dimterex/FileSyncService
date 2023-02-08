namespace TelegramBotService
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using _Interfaces_;

    using NLog;

    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    public class MessageReceivedAction : IReceivedAction
    {
        private const string TAG = nameof(MessageReceivedAction);

        private readonly ConcurrentDictionary<string, ReceavedActionModel> _actions;
        private readonly ILogger _logger;

        public MessageReceivedAction()
        {
            _logger = LogManager.GetLogger(TAG);
            _actions = new ConcurrentDictionary<string, ReceavedActionModel>();

            Configure("/help", "supported command", (bot, message) => Usage(bot, message));
        }

        public void Configure(string message, string comment, ITelegramCommand callBack)
        {
            void customCallBack(ITelegramBotClient botClient, Message msg)
            {
                callBack.Handle();
            }

            if (_actions.TryAdd(message, new ReceavedActionModel(comment, customCallBack)))
                _logger.Info(() => $"{message} registered.");
            else
                _logger.Warn(() => $"{message} doesn't register.");
        }

        public void Execute(ITelegramBotClient botClient, Message message)
        {
            if (message.Type != MessageType.Text)
                return;

            string text = message.Text!.Split(' ')[0];

            if (_actions.TryGetValue(text, out ReceavedActionModel callBack))
            {
                callBack.Action(botClient, message);
                _logger.Debug(() => $"{message} called.");
            }
            else
            {
                _logger.Warn(() => $"{message} doesn't find.");
            }
        }

        internal void Configure(string message, string comment, Action<ITelegramBotClient, Message> callBack)
        {
            if (_actions.TryAdd(message, new ReceavedActionModel(comment, callBack)))
                _logger.Debug(() => $"{message} registered.");
            else
                _logger.Warn(() => $"{message} doesn't register.");
        }

        private async Task<Message> Usage(ITelegramBotClient botClient, Message message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage:");
            foreach (KeyValuePair<string, ReceavedActionModel> receavedActionModel in _actions)
            {
                sb.AppendLine($"{receavedActionModel.Key} - {receavedActionModel.Value.Comment}");
            }

            return await botClient.SendTextMessageAsync(message.Chat.Id, sb.ToString(), replyMarkup: new ReplyKeyboardRemove());
        }
    }
}
