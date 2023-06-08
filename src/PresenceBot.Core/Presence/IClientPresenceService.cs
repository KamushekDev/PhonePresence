using PresenceBot.Core.Presence.Models;

namespace PresenceBot.Core.Presence;

public interface IClientPresenceService
{
    Task<List<PresenceInfo>> GetConnectedClients(CancellationToken token);
}