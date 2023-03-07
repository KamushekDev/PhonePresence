namespace PhonePresenceBot.Core.PhonePresence;

public record PhonePresenceRequest(
    string? PhoneName = null,
    string? PhoneMac = null
)
{
    public bool IsValid() =>
        (PhoneName is not null || PhoneMac is not null) &&
        !(PhoneName is not null && PhoneMac is not null);
};