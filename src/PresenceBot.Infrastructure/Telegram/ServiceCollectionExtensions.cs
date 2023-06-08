using Microsoft.Extensions.DependencyInjection;
using PresenceBot.Infrastructure.BackgroundJobs;
using PresenceBot.Infrastructure.Telegram.Options;

namespace PresenceBot.Infrastructure.Telegram;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegram(this IServiceCollection services,
        Action<TelegramOptions>? configure = null)
    {
        if (configure is not null)
            services.Configure(configure);

        // todo: add interface
        services.AddSingleton<TelegramService>();

        services.AddHostedService<TelegramBackgroundJob>();

        return services;
    }
}