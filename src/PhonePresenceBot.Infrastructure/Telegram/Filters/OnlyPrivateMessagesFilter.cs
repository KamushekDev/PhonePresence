using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PhonePresenceBot.Infrastructure.Telegram.Filters;

public class OnlyPrivateMessagesFilter : ITelegramUpdateFilter
{
    private readonly ILogger<OnlyPrivateMessagesFilter> _logger;

    public OnlyPrivateMessagesFilter(ILogger<OnlyPrivateMessagesFilter> logger)
    {
        _logger = logger;
    }

    public bool ShouldHandle(Update update)
    {
        if (update.Type is not UpdateType.Message)
        {
            _logger.LogDebug("Update with id ({UpdateId}) was skipped because update type is {UpdateType}", update.Id,
                update.Type);
            return false;
        }

        var message = update.Message!;

        if (update.Message?.Type is not MessageType.Text)
        {
            _logger.LogDebug("Update with id ({UpdateId}) was skipped because message type is {MessageType}", update.Id,
                message.Type);
            return false;
        }

        if (message.Chat.Type is not ChatType.Private)
        {
            _logger.LogDebug("Update with id ({UpdateId}) was skipped because chat type is {ChatType}", update.Id,
                message.Chat.Type);
            return false;
        }

        if (message.From is null)
        {
            _logger.LogDebug("Update with id ({UpdateId}) was skipped because sender is null", update.Id);
            return false;
        }

        return true;
    }
}