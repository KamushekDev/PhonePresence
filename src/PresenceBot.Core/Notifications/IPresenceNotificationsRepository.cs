using PresenceBot.Core.Notifications.Models;

namespace PresenceBot.Core.Notifications;

public interface IPresenceNotificationsRepository
{
    Task<List<NotificationRequest>> GetRequests(CancellationToken token);
     
    Task RemoveRequests(List<NotificationRequest> requests, CancellationToken token);
     
    Task AddRequest(NotificationRequest request, CancellationToken token);
}