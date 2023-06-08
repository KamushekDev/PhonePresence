namespace PresenceBot.Core.Presence.Models;

// Can't rely on mac nor on name
public record Client(string? Name, string? Nickname, string Mac, string Ip)
{
    // Nickname -> Name -> Mac
    public string Identity =>
        string.IsNullOrEmpty(Nickname)
            ? string.IsNullOrEmpty(Name)
                ? Mac
                : Name
            : Nickname;
};