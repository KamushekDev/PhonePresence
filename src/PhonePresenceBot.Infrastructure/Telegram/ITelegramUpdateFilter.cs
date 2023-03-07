using Telegram.Bot.Types;

namespace PhonePresenceBot.Infrastructure.Telegram;

public interface ITelegramUpdateFilter
{
    public bool ShouldHandle(Update update);
}