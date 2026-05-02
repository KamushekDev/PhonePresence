using Microsoft.EntityFrameworkCore;
using PresenceBot.Core.Notifications;
using PresenceBot.Core.Notifications.Models;
using PresenceBot.Infrastructure.Database;

namespace PresenceBot.Infrastructure.Notifications;

public class PresenceNotificationsRepository(PresenceContext context) : IPresenceNotificationsRepository
{
    public async Task<List<NotificationRequest>> GetRequests(CancellationToken token)
    {
        return await context
            .NotificationRequests
            .ToListAsync(token);
    }

    public async Task RemoveRequests(List<NotificationRequest> requests, CancellationToken token)
    {
        context
            .NotificationRequests
            .RemoveRange(requests);
        await context.SaveChangesAsync(token);
    }

    public async Task AddRequest(NotificationRequest request, CancellationToken token)
    {
        await context.AddAsync(request, token);
        await context.SaveChangesAsync(token);
    }
}