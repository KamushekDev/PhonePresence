namespace PhonePresenceBot.Integration.Telegram.Options;

public class TelegramOptions
{
    public static string SectionName = "Telegram";

    public required string ApiKey { get; init; }
}