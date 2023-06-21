namespace PresenceBot.Infrastructure;

public class Settings
{
    public const string SectionName = "Settings";

    public RouterSettings Router { get; set; }
    public PresenceSettings Presence { get; set; }
    public TelegramSettings Telegram { get; set; }

    public class RouterSettings
    {
        public required string Uri { get; set; }
        public required string Login { get; set; }
        public required string Password { get; set; }
    }

    public class PresenceSettings
    {
        public bool ShouldCheckWithPing { get; set; }
        public int PingTimeoutMs { get; set; }
    }

    public class TelegramSettings
    {
        public required string ApiKey { get; set; }
    }
}