using Microsoft.Extensions.DependencyInjection;
using PresenceBot.Core.Presence;
using PresenceBot.Infrastructure.BackgroundJobs;
using PresenceBot.Infrastructure.Presence.Options;
using PresenceBot.Integration.Router.DependencyInjection;
using PresenceBot.Integration.Router.Options;

namespace PresenceBot.Infrastructure.Presence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresenceService(this IServiceCollection services)
    {
        services.AddRouterClient();

        services
            .AddOptions<RouterOptions>()
            .BindConfiguration(RouterOptions.SectionName);
        
        services
            .AddOptions<PresenceOptions>()
            .BindConfiguration(PresenceOptions.SectionName);

        services
            .AddHostedService<PresenceInfoHandlerJob>()
            .AddHostedService<PresenceMonitoringBackgroundJob>();

        services.AddTransient<IClientPresenceService, ClientPresenceService>();

        services.AddTransient<IClientPresenceRepository, ClientPresenceRepository>();

        return services;
    }
}