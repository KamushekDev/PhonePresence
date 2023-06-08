using PresenceBot.Integration.Router.Models;

namespace PresenceBot.Integration.Router;

public interface IRouterGateway
{
    Task<string> Login(CancellationToken cancellationToken);
    Task<List<RouterClient>> GetClientList(CancellationToken cancellationToken);
}