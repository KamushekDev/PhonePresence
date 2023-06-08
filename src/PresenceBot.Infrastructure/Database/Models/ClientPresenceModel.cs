namespace PresenceBot.Infrastructure.Database.Models;

public class ClientPresenceModel
{
    public string Identity { get; set; }
    public DateTimeOffset LastPresentedAt { get; set; }

    // For manual use only
    public string?[] IdentityComponents { get; set; }
}