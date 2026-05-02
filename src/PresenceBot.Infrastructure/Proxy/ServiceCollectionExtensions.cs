using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PresenceBot.Infrastructure.Proxy.Options;

namespace PresenceBot.Infrastructure.Proxy;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProxy(this IServiceCollection services)
    {
        services
            .AddOptions<ProxyOptions>()
            .BindConfiguration(ProxyOptions.SectionName);
        
        services
            .AddHttpClient("WithProxy")
            .ConfigurePrimaryHttpMessageHandler((provider) =>
            {
                var options = provider.GetRequiredService<IOptionsSnapshot<ProxyOptions>>().Value;
                var proxy = new WebProxy
                {
                    Address = options.Uri
                };
                return new HttpClientHandler() { Proxy = proxy };
            });

        return services;
    }
}