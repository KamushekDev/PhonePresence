namespace PhonePresenceBot.Core;

public interface IPhonePresenceBot
{
    public Task Run(CancellationToken token);
}