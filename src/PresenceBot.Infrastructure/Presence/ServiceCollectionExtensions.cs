using Microsoft.Extensions.DependencyInjection;
using PresenceBot.Core.Presence;
using PresenceBot.Infrastructure.BackgroundJobs;
using PresenceBot.Infrastructure.Presence.Options;
using PresenceBot.Integration.Router.DependencyInjection;
using PresenceBot.Integration.Router.Options;

namespace PresenceBot.Infrastructure.Presence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresenceService(this IServiceCollection services,
        Action<RouterOptions>? configureRouter = null, Action<PresenceOptions>? configurePresence = null)
    {
        services.AddRouterClient(configureRouter);

        if (configurePresence is not null)
        {
            services.Configure(configurePresence);
        }

        services
            .AddHostedService<PresenceInfoHandlerJob>()
            .AddHostedService<PresenceMonitoringBackgroundJob>();

        services.AddTransient<IClientPresenceService, ClientPresenceService>();

        services.AddTransient<IClientPresenceRepository, ClientPresenceRepository>();

        return services;
    }
}