using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Options;
using PresenceBot.Core.Presence;
using PresenceBot.Core.Presence.Models;
using PresenceBot.Infrastructure.Presence.Options;
using PresenceBot.Integration.Router;

namespace PresenceBot.Infrastructure.Presence;

public class ClientPresenceGetterService : IClientPresenceGetterService
{
    private readonly IRouterGateway _routerGateway;
    private readonly PresenceOptions _options;

    public ClientPresenceGetterService(IRouterGateway routerGateway, IOptionsSnapshot<PresenceOptions> options)
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

        var lockObj = new Lock();

        await Parallel.ForEachAsync(clients, token, async (client, ct) =>
        {
            var ping = await AvailableForPing(client, ct);
            var pingedClient = new PresenceInfo()
            {
                Client = client,
                Moment = DateTimeOffset.Now,
                Available = ping
            };
            lock (lockObj)
            {
                pingedClients.Add(pingedClient);
            }
        });

        return pingedClients;
    }

    private async Task<bool> AvailableForPing(Client client, CancellationToken token)
    {
        if (!_options.ShouldCheckWithPing)
            return true;

        var ping = new Ping();
        var result = await ping.SendPingAsync(
            IPAddress.Parse(client.Ip),
            TimeSpan.FromMilliseconds(_options.PingTimeoutMs),
            cancellationToken: token);
        return result.Status == IPStatus.Success;
    }
}