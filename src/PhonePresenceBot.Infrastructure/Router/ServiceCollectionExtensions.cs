using Microsoft.Extensions.DependencyInjection;
using PhonePresenceBot.Integration.Router.DependencyInjection;

namespace PhonePresenceBot.Infrastructure.Router;

public static class ServiceCollectionExtensions
{
    public static void AddRouter(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddRouterClient();
    }
}