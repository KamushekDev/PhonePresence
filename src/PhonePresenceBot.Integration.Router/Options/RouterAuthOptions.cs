namespace PhonePresenceBot.Integration.Router.Options;

public class RouterAuthOptions
{
    public const string SectionName = "RouterAuthOptions";
    
    public required string Login { get; init; }
    public required string Password { get; init; }
}