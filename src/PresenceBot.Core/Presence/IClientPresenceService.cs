using PresenceBot.Core.Presence.Models;

namespace PresenceBot.Core.Presence;

public interface IClientPresenceService
{
    Task<ClientPresenceInfo> GetIdentityPresence(string clientIdentity, TimeSpan confidenceInterval,
        CancellationToken token);
}