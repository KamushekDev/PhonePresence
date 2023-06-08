namespace PresenceBot.Integration.Router.Options;

public class RouterOptions
{
    public required Uri RouterUri { get; set; }
    public required string Login { get; set; }
    public required string Password { get; set; }
}