using System.Collections.Concurrent;
using Core.Logger;
using Core.Logger._Enums_;
using Core.Logger._Interfaces_;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotService._Interfaces_;

namespace TelegramBotService
{
    public class ActionsService
    {
        private const string TAG = nameof(ActionsService);
        private readonly ConcurrentDictionary<UpdateType, IReceivedAction> _actions;
        private readonly ILoggerService _loggerService;

        public ActionsService(ILoggerService loggerService)
        {
            _loggerService = loggerService;
            _actions = new ConcurrentDictionary<UpdateType, IReceivedAction>();
            var messageReceivedAction = new MessageReceivedAction(_loggerService);
            _actions.TryAdd(UpdateType.Message, messageReceivedAction);
        }

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            if (_actions.TryGetValue(update.Type, out var action))
                action.Execute(botClient, update.Message ?? update.EditedMessage);
            else
                _loggerService.SendLog(LogLevel.Warning, TAG, () => $"{update.Type} doesn't supported.");
        }

        public void Configure(string message, string comment, ITelegramCommand callback)
        {
            if (_actions.TryGetValue(UpdateType.Message, out var action))
                action.Configure(message, comment, callback);
            else
                _loggerService.SendLog(LogLevel.Warning, TAG, () => $"{UpdateType.Message} doesn't supported.");
        }
    }
}