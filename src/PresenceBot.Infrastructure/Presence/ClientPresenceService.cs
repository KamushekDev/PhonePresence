using PresenceBot.Core.Presence;
using PresenceBot.Core.Presence.Models;

namespace PresenceBot.Infrastructure.Presence;

public class ClientPresenceService(IClientPresenceRepository repository) : IClientPresenceService
{
    public async Task<ClientPresenceInfo> GetIdentityPresence(string clientIdentity, TimeSpan confidenceInterval,
        CancellationToken token)
    {
        var info = await repository.FindByIdentity(clientIdentity, token);
        if (info is null)
            return new ClientWasNeverPresentedResponse()
            {
                ClientIdentity = clientIdentity
            };

        if ((DateTimeOffset.Now - info.LastAvailableAt) > confidenceInterval)
            return new ClientWasPresentedResponse()
            {
                ClientIdentity = clientIdentity,
                LastPresentedAt = info.LastAvailableAt
            };

        return new ClientPresentedResponse(){ClientIdentity = clientIdentity};
    }
}