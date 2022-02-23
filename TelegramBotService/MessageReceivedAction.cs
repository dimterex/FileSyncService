using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotService._Interfaces_;

namespace TelegramBotService
{
    public class MessageReceivedAction : IReceivedAction
    {
        private readonly ConcurrentDictionary<string, ReceavedActionModel> _actions;
        private readonly ILogger _logger;

        public MessageReceivedAction()
        {
            _actions = new ConcurrentDictionary<string, ReceavedActionModel>();
            _logger = LogManager.GetCurrentClassLogger();

            Configure("/help", "supported command", (bot, message) => Usage(bot, message));
        }

        internal void Configure(string message, string comment, Action<ITelegramBotClient, Message> callBack)
        {
            if (_actions.TryAdd(message, new ReceavedActionModel(comment, callBack)))
            {
                _logger.Info(() => $"{message} registered.");
            }
            else
            {
                _logger.Warn(() => $"{message} doesn't register.");
            }
        }

        public void Configure(string message, string comment, ITelegramCommand callBack)
        {
            void customCallBack (ITelegramBotClient botClient, Message msg)
            {
                callBack.Handle();
            }
            
            if (_actions.TryAdd(message, new ReceavedActionModel(comment, customCallBack)))
            {
                _logger.Info(() => $"{message} registered.");
            }
            else
            {
                _logger.Warn(() => $"{message} doesn't register.");
            }
        }

        public void Execute(ITelegramBotClient botClient, Message message)
        {
            if (message.Type != MessageType.Text)
                return;
            
            var text = message.Text!.Split(' ')[0];
            
            if (_actions.TryGetValue(text, out var callBack))
            {
                callBack.Action(botClient, message);
                _logger.Debug(() => $"{message} called.");
            }
            else
            {
                _logger.Warn(() => $"{message} doesn't find.");
            }
        }
        
        private async Task<Message> Usage(ITelegramBotClient botClient, Message message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage:");
            foreach (var receavedActionModel in _actions)
            {
                sb.AppendLine($"{receavedActionModel.Key} - {receavedActionModel.Value.Comment}");
            }

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                text: sb.ToString(),
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}