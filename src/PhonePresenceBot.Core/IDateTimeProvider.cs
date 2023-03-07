namespace PhonePresenceBot.Core;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow();
}