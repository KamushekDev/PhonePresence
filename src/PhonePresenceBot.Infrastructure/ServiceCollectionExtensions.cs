using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhonePresenceBot.Core;
using PhonePresenceBot.Infrastructure.PhonePresence;
using PhonePresenceBot.Infrastructure.Router;
using PhonePresenceBot.Infrastructure.Telegram;
using Serilog;

namespace PhonePresenceBot.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton(configuration);

        serviceCollection.AddLogging(configuration);

        serviceCollection.AddTransient<IPhonePresenceBot, PhonePresenceBot>();
        serviceCollection.AddTransient<IDateTimeProvider, DateTimeProvider>();

        serviceCollection.AddRouter();

        serviceCollection.AddPhonePresence();

        serviceCollection.AddTelegram();
    }

    private static void AddLogging(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var serilogLogger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration, "Serilog")
            .CreateLogger();

        serviceCollection.AddLogging(x => x.AddSerilog(serilogLogger, dispose: true));
    }
}