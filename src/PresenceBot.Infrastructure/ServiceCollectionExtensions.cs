using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PresenceBot.Infrastructure.Database;
using PresenceBot.Infrastructure.MessageBus;
using PresenceBot.Infrastructure.Messages;
using PresenceBot.Infrastructure.Notifications;
using PresenceBot.Infrastructure.Presence;
using PresenceBot.Infrastructure.Proxy;
using PresenceBot.Infrastructure.Telegram;
using PresenceBot.Infrastructure.VK;

namespace PresenceBot.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);

        services.AddPresenceService();

        services
            .AddTelegram()
            .AddVkontakte()
            .AddMessageBus()
            .AddProxy()
            .AddMessageFormatter()
            .AddPresenceNotifications()
            ;

        services
            .AddSingleton(TimeProvider.System);
        
        return services;
    }
}