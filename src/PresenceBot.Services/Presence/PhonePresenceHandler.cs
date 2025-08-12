using Comandante;
using OneOf;
using PresenceBot.Core.Presence;

namespace PresenceBot.Services.Presence;

public class PhonePresenceHandler : IQueryHandler<PhonePresenceHandler.Query, OneOf<
    PhonePresenceHandler.ClientPresentedResponse, PhonePresenceHandler.ClientWasPresentedResponse,
    PhonePresenceHandler.ClientWasNeverPresentedResponse>>
{
    private readonly IClientPresenceRepository _repository;

    public PhonePresenceHandler(IClientPresenceRepository repository)
    {
        _repository = repository;
    }

    public async Task<OneOf<ClientPresentedResponse, ClientWasPresentedResponse, ClientWasNeverPresentedResponse>>
        Handle(Query query, CancellationToken token)
    {
        const string PhoneIdentity = "Kirill-s-S25-Ultra";
        TimeSpan onlineThreshold = TimeSpan.FromMinutes(2);

        // todo: filter by userid

        // todo: get current phone name from config/db
        var clientName = PhoneIdentity;

        var info = await _repository.FindByIdentity(clientName, token);
        if (info is null)
            return new ClientWasNeverPresentedResponse()
            {
                ClientIdentity = clientName
            };

        if ((DateTimeOffset.Now - info.LastAvailableAt) > onlineThreshold)
            return new ClientWasPresentedResponse()
            {
                ClientIdentity = clientName,
                LastPresentedAt = info.LastAvailableAt
            };

        return new ClientPresentedResponse()
        {
            ClientIdentity = clientName,
        };
    }

    public class
        Query : IQuery<OneOf<ClientPresentedResponse, ClientWasPresentedResponse, ClientWasNeverPresentedResponse>>
    {
        public long FromUserId { get; init; }
    }

    public sealed class ClientPresentedResponse
    {
        public required string ClientIdentity { get; init; }
    }

    public sealed class ClientWasPresentedResponse
    {
        public required string ClientIdentity { get; init; }
        public required DateTimeOffset LastPresentedAt { get; init; }

        public TimeSpan ElapsedTime => DateTimeOffset.Now - LastPresentedAt;
    }

    public sealed class ClientWasNeverPresentedResponse
    {
        public required string ClientIdentity { get; init; }
    }
}