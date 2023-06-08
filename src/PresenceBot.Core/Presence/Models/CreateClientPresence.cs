namespace PresenceBot.Core.Presence.Models;

public class CreateClientPresence
{
    public required string Identity { get; init; }
    public required DateTimeOffset LastAvailableAt { get; init; }
    public string?[] IdentityComponents { get; init; }
}