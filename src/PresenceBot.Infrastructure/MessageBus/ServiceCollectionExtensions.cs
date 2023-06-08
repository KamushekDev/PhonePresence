using Microsoft.Extensions.DependencyInjection;
using PresenceBot.Core.MessageBus;
using PresenceBot.Core.Presence.Models;

namespace PresenceBot.Infrastructure.MessageBus;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        const int presenceBusCapacity = 100;

        services.AddSingleton<IMessageBus<PresenceInfo>, MessageBus<PresenceInfo>>(
            _ => new MessageBus<PresenceInfo>(presenceBusCapacity)
        );

        return services;
    }
}