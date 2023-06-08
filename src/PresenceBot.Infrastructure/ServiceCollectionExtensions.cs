using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PresenceBot.Infrastructure.Database;
using PresenceBot.Infrastructure.MessageBus;
using PresenceBot.Infrastructure.Presence;
using PresenceBot.Infrastructure.Telegram;
using PresenceBot.Services;

namespace PresenceBot.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);

        var settings = configuration.GetSection(Settings.SectionName).Get<Settings>();

        // todo: real validation    and exception
        if (settings is null)
            throw new Exception("");

        services.AddPresenceService(
            configureRouter: options =>
            {
                options.RouterUri = new Uri(settings.Router.Uri);
                options.Login = settings.Router.Login;
                options.Password = settings.Router.Password;
            },
            configurePresence: options =>
            {
                options.ShouldCheckWithPing = settings.Presence.ShouldCheckWithPing;
                options.PingTimeoutMs = settings.Presence.PingTimeoutMs;
            });

        services.AddMessageBus();

        services
            .AddTelegram(configure: options =>
            {
                // do not inline this, RIDER 
                options.ApiKey = settings.Telegram.ApiKey;
            })
            .AddServices();

        return services;
    }
}

public class Settings
{
    public const string SectionName = "Settings";

    public RouterSettings Router { get; set; }
    public PresenceSettings Presence { get; set; }
    public TelegramSettings Telegram { get; set; }

    public class RouterSettings
    {
        public required string Uri { get; set; }
        public required string Login { get; set; }
        public required string Password { get; set; }
    }

    public class PresenceSettings
    {
        public bool ShouldCheckWithPing { get; set; }
        public int PingTimeoutMs { get; set; }
    }

    public class TelegramSettings
    {
        public required string ApiKey { get; set; }
    }
}