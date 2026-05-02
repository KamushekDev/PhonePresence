namespace PresenceBot.Infrastructure.Presence.Options;

public class PresenceOptions
{
    public const string SectionName = "Settings:Presence";
    public bool ShouldCheckWithPing { get; set; }
    public int PingTimeoutMs { get; set; }
    
    public string WantedClientIdentity { get; set; }
    public TimeSpan WantedConfidenceInterval { get; set; } = TimeSpan.FromMinutes(5);
}