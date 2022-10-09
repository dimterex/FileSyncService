using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Logger;
using Core.Logger._Enums_;
using Core.Logger._Interfaces_;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using TelegramBotService._Interfaces_;

namespace TelegramBotService
{
    public class TelegramService : ITelegramService
    {
        private const string TAG = nameof(TelegramService);
        private readonly ActionsService _actionsService;
        private readonly ILoggerService _loggerService;
        private readonly ITelegramBotClient _telegramBotClient;

        private readonly int _telegramId;
        private readonly CancellationTokenSource _cancellationToketSource;

        public TelegramService(string botToken, int telegram_id, ILoggerService loggerService)
        {
            _telegramId = telegram_id;
            _loggerService = loggerService;
            _telegramBotClient = new TelegramBotClient(botToken);

            _actionsService = new ActionsService(_loggerService);
            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new();
            _cancellationToketSource = new CancellationTokenSource();
            _telegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, _cancellationToketSource.Token); 
        }

        public Task SendTextMessageAsync(string message)
        {
            return _telegramBotClient.SendTextMessageAsync(_telegramId, message);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            await Task.Run(() => { _actionsService.HandleUpdateAsync(botClient, update); }, cancellationToken);
        }

        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var errorMessage = exception switch
                {
                    ApiRequestException apiRequestException =>
                        $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                _loggerService.SendLog(LogLevel.Error, TAG, () => errorMessage);
            }, cancellationToken);
        }

        public void Configure(string message, string comment, ITelegramCommand action)
        {
            _actionsService.Configure(message, comment, action);
        }
    }
}