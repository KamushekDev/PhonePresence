using Telegram.Bot;
using Telegram.Bot.Types;

namespace PhonePresenceBot.Integration.Telegram;

public interface ITelegramUpdateHandler
{
    public Task Handle(ITelegramBotClient client, Update update, CancellationToken token);
}