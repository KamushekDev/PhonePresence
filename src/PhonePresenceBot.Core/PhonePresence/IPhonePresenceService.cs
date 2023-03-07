namespace PhonePresenceBot.Core.PhonePresence;

public interface IPhonePresenceService
{
    public Task<bool> IsPhonePresented(PhonePresenceRequest request, CancellationToken token);
}