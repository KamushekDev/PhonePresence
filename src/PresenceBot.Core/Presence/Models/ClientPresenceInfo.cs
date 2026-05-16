using OneOf;

namespace PresenceBot.Core.Presence.Models;

[GenerateOneOf]
public partial class ClientPresenceInfo
    : OneOfBase<ClientPresentedResponse, ClientWasPresentedResponse, ClientWasNeverPresentedResponse>;
    
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