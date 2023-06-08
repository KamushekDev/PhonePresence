namespace PresenceBot.Infrastructure.Telegram.Options;

public class TelegramOptions
{
    public const string SectionName = "Telegram";

    public required string ApiKey { get; set; }
}