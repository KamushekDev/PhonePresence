using Microsoft.Extensions.DependencyInjection;
using PresenceBot.Infrastructure.BackgroundJobs;
using PresenceBot.Infrastructure.VK.Options;

namespace PresenceBot.Infrastructure.VK;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVkontakte(this IServiceCollection services)
    {
        services
            .AddOptions<VkOptions>()
            .BindConfiguration(VkOptions.SectionName);

        services
            .AddHostedService<VkBackgroundJob>();
        
        return services;
    }
}