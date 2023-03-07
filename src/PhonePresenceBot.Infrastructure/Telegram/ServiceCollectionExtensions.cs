using Microsoft.Extensions.DependencyInjection;
using PhonePresenceBot.Infrastructure.Telegram.Filters;
using PhonePresenceBot.Infrastructure.Telegram.Handlers;
using PhonePresenceBot.Integration.Telegram.DependencyInjection;

namespace PhonePresenceBot.Infrastructure.Telegram;

public static class ServiceCollectionExtensions
{
    public static void AddTelegram(this IServiceCollection serviceCollection)
    {
          serviceCollection.AddFilters();

          serviceCollection.AddHandlers();
          
          serviceCollection
              .AddTelegramPipeline()
              .AddHandler<PhonePresenceHandler>()
              .Build();
    }

    private static void AddFilters(this IServiceCollection sc)
    {
        sc.AddTransient<OnlyPrivateMessagesFilter>();
        sc.AddTransient<OldMessagesFilter>();
    }
    
    private static void AddHandlers(this IServiceCollection sc)
    {
        sc.AddTransient<PhonePresenceHandler>();
    }
}