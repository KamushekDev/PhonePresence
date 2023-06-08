using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PresenceBot.Integration.Router.Options;

namespace PresenceBot.Integration.Router.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private const string HttpClientName = "RouterGatewayClient";

    public static void AddRouterClient(this IServiceCollection serviceCollection,
        Action<RouterOptions>? configure = null)
    {
        if (configure != null)
            serviceCollection.Configure(configure);

        serviceCollection.AddHttpClient<RouterGateway>(HttpClientName, ConfigureHttpClientForRouter)
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler()
                {
                    UseCookies = true
                }
            );

        serviceCollection.AddTransient<IRouterGateway, RouterGateway>(RouterGatewayFactory);
    }

    private static RouterGateway RouterGatewayFactory(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<RouterOptions>>().Value;
        var factory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        var client = factory.CreateClient(HttpClientName);

        return new RouterGateway(options, client);
    }

    private static void ConfigureHttpClientForRouter(IServiceProvider serviceProvider, HttpClient client)
    {
        using var scope = serviceProvider.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<RouterOptions>>();
        RouterHelper.ConfigureHttpClient(client, options.Value);
    }
}