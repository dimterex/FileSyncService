namespace TelegramBotService
{
    using System.Collections.Concurrent;

    using _Interfaces_;

    using NLog;

    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class ActionsService
    {
        private const string TAG = nameof(ActionsService);
        private readonly ConcurrentDictionary<UpdateType, IReceivedAction> _actions;
        private readonly ILogger _logger;

        public ActionsService()
        {
            _logger = LogManager.GetLogger(TAG);
            _actions = new ConcurrentDictionary<UpdateType, IReceivedAction>();
            var messageReceivedAction = new MessageReceivedAction();
            _actions.TryAdd(UpdateType.Message, messageReceivedAction);
        }

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            if (_actions.TryGetValue(update.Type, out IReceivedAction action))
                action.Execute(botClient, update.Message ?? update.EditedMessage);
            else
                _logger.Warn(() => $"{update.Type} doesn't supported.");
        }

        public void Configure(string message, string comment, ITelegramCommand callback)
        {
            if (_actions.TryGetValue(UpdateType.Message, out IReceivedAction action))
                action.Configure(message, comment, callback);
            else
                _logger.Warn(() => $"{UpdateType.Message} doesn't supported.");
        }
    }
}
