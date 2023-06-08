namespace PresenceBot.Infrastructure.Presence.Options;

public class PresenceOptions
{
    public bool ShouldCheckWithPing { get; set; }
    public int PingTimeoutMs { get; set; }
}