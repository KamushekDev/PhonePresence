using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Options;
using PresenceBot.Core.Presence;
using PresenceBot.Core.Presence.Models;
using PresenceBot.Infrastructure.Presence.Options;
using PresenceBot.Integration.Router;

namespace PresenceBot.Infrastructure.Presence;

public class ClientPresenceService : IClientPresenceService
{
    private readonly IRouterGateway _routerGateway;
    private readonly PresenceOptions _options;

    public ClientPresenceService(IRouterGateway routerGateway, IOptionsSnapshot<PresenceOptions> options)
    {
        _routerGateway = routerGateway;
        _options = options.Value;
    }

    public async Task<List<PresenceInfo>> GetConnectedClients(CancellationToken token)
    {
        await _routerGateway.Login(token);

        var routerClients = await _routerGateway.GetClientList(token);

        var clients = routerClients.Select(x => new Client(x.Name, x.NickName, x.Mac, x.Ip)).ToList();

        var pingedClients = new List<PresenceInfo>(routerClients.Count);

        // todo: parallel
        foreach (var client in clients)
        {
            var ping = await AvailableForPing(client);
            var pingedClient = new PresenceInfo()
            {
                Client = client,
                Moment = DateTimeOffset.Now,
                Available = ping
            };
            pingedClients.Add(pingedClient);
        }

        return pingedClients;
    }

    private async Task<bool> AvailableForPing(Client client)
    {
        if (!_options.ShouldCheckWithPing)
            return true;

        var ping = new Ping();
        var result = await ping.SendPingAsync(IPAddress.Parse(client.Ip), _options.PingTimeoutMs);
        return result.Status == IPStatus.Success;
    }
}