namespace PhonePresenceBot.Infrastructure.Options;

public class Settings
{
    public const string SectionName = "Settings";

    public required long[] ProhibitedUserIds { get; init; } = Array.Empty<long>();
    public required string PhoneName { get; init; }
}