namespace PresenceBot.Integration.Router.Exceptions;

public class RouterLoginFailed : Exception
{
    public RouterLoginFailed(string message, Exception? exception = null) : base(message, exception) { }
}