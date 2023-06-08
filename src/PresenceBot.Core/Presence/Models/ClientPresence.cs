namespace PresenceBot.Core.Presence.Models;

public class ClientPresence
{
    public required string Identity { get; init; }
    public required DateTimeOffset LastAvailableAt { get; init; }
}