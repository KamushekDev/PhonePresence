namespace PresenceBot.Integration.Router.Options;

public class RouterOptions
{
    public const string SectionName = "Settings:Router";
    public Uri Uri { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}