namespace PresenceBot.Core.Presence.Models;

public class PresenceInfo
{
    public required Client Client { get; init; }
    public required DateTimeOffset Moment { get; init; }
    public required bool Available { get; init; }
}