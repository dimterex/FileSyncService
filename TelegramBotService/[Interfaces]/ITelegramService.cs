﻿namespace TelegramBotService._Interfaces_
{
    using System.Threading.Tasks;

    public interface ITelegramService
    {
        Task SendTextMessageAsync(string message);
    }
}
