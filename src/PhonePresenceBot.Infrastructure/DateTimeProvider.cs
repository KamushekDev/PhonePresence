using PhonePresenceBot.Core;

namespace PhonePresenceBot.Infrastructure;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;
}