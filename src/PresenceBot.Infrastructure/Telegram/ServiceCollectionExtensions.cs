using Microsoft.Extensions.DependencyInjection;
using PresenceBot.Infrastructure.BackgroundJobs;
using PresenceBot.Infrastructure.Telegram.Options;

namespace PresenceBot.Infrastructure.Telegram;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegram(this IServiceCollection services)
    {
        services
            .AddOptions<TelegramOptions>()
            .BindConfiguration(TelegramOptions.SectionName);
        
        services.AddHostedService<TelegramBackgroundJob>();

        return services;
    }
}