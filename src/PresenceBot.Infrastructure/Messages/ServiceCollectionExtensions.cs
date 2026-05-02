using Microsoft.Extensions.DependencyInjection;
using PresenceBot.Core.Messages;

namespace PresenceBot.Infrastructure.Messages;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageFormatter(this IServiceCollection services)
    {
        services
            .AddSingleton<IMessageFormatter, MessageFormatter>();
        
        return services;
    }
}