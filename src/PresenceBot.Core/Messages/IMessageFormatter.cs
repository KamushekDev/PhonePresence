namespace PresenceBot.Core.Messages;

public interface IMessageFormatter
{
    string GetActiveClientMessage();
    string GetInactiveClientMessage(TimeSpan inactivityTimeSpan);
    string GetNeverActiveClient(string clientIdentity);
}