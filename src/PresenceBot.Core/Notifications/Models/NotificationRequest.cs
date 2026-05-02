namespace PresenceBot.Core.Notifications.Models;

public class NotificationRequest
{
    public long Id { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    
    public required string ClientIdentity { get; init; }
    public required NotificationSource Source { get; init; }
    public required string ReplyData { get; init; }
}