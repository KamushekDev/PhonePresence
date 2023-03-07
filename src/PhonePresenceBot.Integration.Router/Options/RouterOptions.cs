namespace PhonePresenceBot.Integration.Router.Options;

public class RouterOptions
{
    public const string SectionName = "RouterOptions";
    
    public required Uri RouterUri { get; init; }
}