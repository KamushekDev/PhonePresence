using Telegram.Bot;
using Telegram.Bot.Types;

namespace PhonePresenceBot.Integration.Telegram.DependencyInjection;

// ReSharper disable once InconsistentNaming
public interface ITelegramUpdateHandler
{
    public Task Handle(ITelegramBotClient client, Update update, CancellationToken token);
}