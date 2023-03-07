using PhonePresenceBot.Core;
using Telegram.Bot.Types;

namespace PhonePresenceBot.Infrastructure.Telegram.Filters;

public class OldMessagesFilter : ITelegramUpdateFilter
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public OldMessagesFilter(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public bool ShouldHandle(Update update)
    {
        if (update.Message is null)
            return true;

        var currentTime = _dateTimeProvider.UtcNow();

        var messageTime = update.Message.Date;

        var timeDiff = currentTime - messageTime;

        return timeDiff.TotalMinutes <= 5;
    }
}