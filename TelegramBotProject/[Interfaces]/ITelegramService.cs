using System.Threading.Tasks;

namespace TelegramBotProject._Interfaces_
{
    public interface ITelegramService
    {
        Task SendTextMessageAsync(string message);
    }
}