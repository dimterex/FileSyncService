using System.Threading.Tasks;

namespace TelegramBotService._Interfaces_
{
    public interface ITelegramService
    {
        Task SendTextMessageAsync(string message);
    }
}