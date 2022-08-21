using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Core.Logger;
using Core.Logger._Enums_;
using Core.Logger._Interfaces_;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotService._Interfaces_;

namespace TelegramBotService
{
    public class MessageReceivedAction : IReceivedAction
    {
        private const string TAG = nameof(MessageReceivedAction);

        private readonly ConcurrentDictionary<string, ReceavedActionModel> _actions;
        private readonly ILoggerService _loggerService;

        public MessageReceivedAction(ILoggerService loggerService)
        {
            _loggerService = loggerService;
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
                _loggerService.SendLog(LogLevel.Info, TAG, () => $"{message} registered.");
            else
                _loggerService.SendLog(LogLevel.Warning, TAG, () => $"{message} doesn't register.");
        }

        public void Execute(ITelegramBotClient botClient, Message message)
        {
            if (message.Type != MessageType.Text)
                return;

            var text = message.Text!.Split(' ')[0];

            if (_actions.TryGetValue(text, out var callBack))
            {
                callBack.Action(botClient, message);
                _loggerService.SendLog(LogLevel.Debug, TAG, () => $"{message} called.");
            }
            else
            {
                _loggerService.SendLog(LogLevel.Warning, TAG, () => $"{message} doesn't find.");
            }
        }

        internal void Configure(string message, string comment, Action<ITelegramBotClient, Message> callBack)
        {
            if (_actions.TryAdd(message, new ReceavedActionModel(comment, callBack)))
                _loggerService.SendLog(LogLevel.Info, TAG, () => $"{message} registered.");
            else
                _loggerService.SendLog(LogLevel.Warning, TAG, () => $"{message} doesn't register.");
        }

        private async Task<Message> Usage(ITelegramBotClient botClient, Message message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage:");
            foreach (var receavedActionModel in _actions)
                sb.AppendLine($"{receavedActionModel.Key} - {receavedActionModel.Value.Comment}");

            return await botClient.SendTextMessageAsync(message.Chat.Id,
                sb.ToString(),
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}