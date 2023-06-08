using PresenceBot.Core.Presence.Models;

namespace PresenceBot.Core.Presence;

public interface IClientPresenceRepository
{
    Task AddClientPresence(CreateClientPresence request, CancellationToken token);

    Task<ClientPresence?> UpdateLastAvailableAt(string identity, DateTimeOffset newValue,
        CancellationToken token);

    Task<ClientPresence?> FindByIdentity(string identity, CancellationToken token);
    Task<List<ClientPresence>> GetClients(CancellationToken token);
}