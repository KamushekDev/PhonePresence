using Comandante;
using Microsoft.Extensions.DependencyInjection;

namespace PresenceBot.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddComandate(typeof(IServicesAssemblyMarker).Assembly);

        return services;
    }
}