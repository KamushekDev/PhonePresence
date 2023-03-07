namespace PhonePresenceBot.Core.PhoneMonitoring;

public class PhonePresenceInfo
{
    public PhonePresence State { get; }

    public DateTimeOffset? LastSeen { get; }

    private PhonePresenceInfo(PhonePresence state, DateTimeOffset? lastSeen)
    {
        State = state;
        LastSeen = lastSeen;
    }

    public PhonePresenceInfo NotPresented() => new(PhonePresence.NotPresented, null);
    public PhonePresenceInfo WasPresented(DateTimeOffset lastSeen) => new(PhonePresence.PhoneWasPresented, lastSeen);
    public PhonePresenceInfo IsPresented() => new(PhonePresence.PhonePresented, null);
}

public enum PhonePresence
{
    NotPresented,
    PhoneWasPresented,
    PhonePresented
}