using System;
using System.Collections.Concurrent;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotService._Interfaces_;

namespace TelegramBotService
{
    public class ActionsService
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<UpdateType, IReceivedAction> _actions;

        public ActionsService()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _actions = new ConcurrentDictionary<UpdateType, IReceivedAction>();

            
            var messageReceivedAction = new MessageReceivedAction();
            _actions.TryAdd(UpdateType.Message, messageReceivedAction);
        }


        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            if (_actions.TryGetValue(update.Type, out var action))
            {
                action.Execute(botClient, update.Message ?? update.EditedMessage);
            }
            else
            {
                _logger.Warn(() => $"{update.Type} doesn't supported.");
            }
        }

        public void Configure(string message, string comment, ITelegramCommand callback)
        {
            if (_actions.TryGetValue(UpdateType.Message, out var action))
            {
                action.Configure(message, comment, callback);
            }
            else
            {
                _logger.Warn(() => $"{UpdateType.Message} doesn't supported.");
            }
        }
    }
}