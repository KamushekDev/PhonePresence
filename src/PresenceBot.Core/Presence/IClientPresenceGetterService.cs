using PresenceBot.Core.Presence.Models;

namespace PresenceBot.Core.Presence;

public interface IClientPresenceGetterService
{
    Task<List<PresenceInfo>> GetConnectedClients(CancellationToken token);
}