using PhonePresenceBot.Core.PhonePresence;
using PhonePresenceBot.Integration.Router;

namespace PhonePresenceBot.Infrastructure.PhonePresence;

public class PhonePresenceService : IPhonePresenceService
{
    private readonly RouterGateway _gateway;

    public PhonePresenceService(RouterGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task<bool> IsPhonePresented(PhonePresenceRequest request, CancellationToken token)
    {
        if (!request.IsValid())
            throw new InvalidOperationException("Request isn't valid");

        await _gateway.Login(token);

        var clients = await _gateway.GetClientList(token);

        var phone = clients.FirstOrDefault(x =>
            x.Mac == request.PhoneMac ||
            x.Name == request.PhoneName ||
            x.NickName == request.PhoneName
        );

        return phone is not null;
    }
}