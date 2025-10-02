namespace TelegramBotService
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using _Interfaces_;

    using NLog;

    using Telegram.Bot;
    using Telegram.Bot.Exceptions;
    using Telegram.Bot.Extensions.Polling;
    using Telegram.Bot.Types;

    public class TelegramService : ITelegramService
    {
        private const string TAG = nameof(TelegramService);
        private readonly ActionsService _actionsService;
        private readonly CancellationTokenSource _cancellationToketSource;
        private readonly ILogger _logger;
        private readonly ITelegramBotClient _telegramBotClient;

        public TelegramService(string botToken)
        {
            _logger = LogManager.GetLogger(TAG);
            _telegramBotClient = new TelegramBotClient(botToken);

            _actionsService = new ActionsService();
            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new();
            _cancellationToketSource = new CancellationTokenSource();
            _telegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, _cancellationToketSource.Token);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await Task.Run(
                () =>
                {
                    _actionsService.HandleUpdateAsync(botClient, update);
                },
                cancellationToken);
        }

        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await Task.Run(
                () =>
                {
                    string errorMessage = exception switch
                    {
                        ApiRequestException apiRequestException =>
                            $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                        _ => exception.ToString()
                    };

                    _logger.Error(() => errorMessage);
                },
                cancellationToken);
        }

        public void Configure(string message, string comment, ITelegramCommand action)
        {
            _actionsService.Configure(message, comment, action);
        }
    }
}
