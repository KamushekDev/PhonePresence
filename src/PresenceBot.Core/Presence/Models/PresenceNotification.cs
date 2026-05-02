using OneOf;

namespace PresenceBot.Core.Presence.Models;

[GenerateOneOf]
public partial class PresenceNotification : OneOfBase<ClientBecameActiveNotification, ClientFirstAppearanceNotification>
{
}

public record ClientBecameActiveNotification(
    string ClientIdentity,
    DateTimeOffset BecameActiveAt,
    TimeSpan InactivityPeriod);

public record ClientFirstAppearanceNotification(string ClientIdentity);