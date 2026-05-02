using Microsoft.Extensions.DependencyInjection;
using PresenceBot.Core.Notifications;

namespace PresenceBot.Infrastructure.Notifications;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresenceNotifications(this IServiceCollection services)
    {
        services.AddTransient<IPresenceNotificationsRepository, PresenceNotificationsRepository>();

        return services;
    }
}