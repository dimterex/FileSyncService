using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using TelegramBotService._Interfaces_;

namespace TelegramBotService
{
    public class TelegramService : ITelegramService
    {
        private readonly int _telegramId;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly ActionsService _actionsService;
        private readonly ILogger _logger;

        public TelegramService(string botToken, int telegram_id)
        {
            _logger = LogManager.GetCurrentClassLogger();
            
            _telegramId = telegram_id;
            _telegramBotClient = new TelegramBotClient(botToken);

            _actionsService = new ActionsService();
            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
            
            using var cts = new CancellationTokenSource();
            {
                _telegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                _actionsService.HandleUpdateAsync(botClient, update);
            }, cancellationToken);
        }
        
        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var errorMessage = exception switch
                {
                    ApiRequestException apiRequestException =>
                        $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                _logger.Error(() => errorMessage);
            }, cancellationToken);
        }

        public Task SendTextMessageAsync(string message)
        {
            return _telegramBotClient.SendTextMessageAsync(_telegramId, message);
        }

        public void Configure(string message, string comment, ITelegramCommand action)
        {
            _actionsService.Configure(message, comment, action);
        }
    }
}