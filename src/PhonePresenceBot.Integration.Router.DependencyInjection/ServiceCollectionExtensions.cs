using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PhonePresenceBot.Integration.Router.Options;

namespace PhonePresenceBot.Integration.Router.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private const string HttpClientName = "RouterGatewayClient";

    public static void AddRouterClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<RouterOptions>()
            .BindConfiguration(RouterOptions.SectionName);

        serviceCollection.AddOptions<RouterAuthOptions>()
            .BindConfiguration(RouterAuthOptions.SectionName);

        serviceCollection.AddHttpClient<RouterGateway>(HttpClientName, ConfigureHttpClientForRouter)
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler()
                {
                    UseCookies = true
                }
            );

        serviceCollection.AddTransient<RouterGateway>(RouterGatewayFactory);
    }

    private static RouterGateway RouterGatewayFactory(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<RouterAuthOptions>>().Value;
        var factory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        var client = factory.CreateClient(HttpClientName);

        return new RouterGateway(options, client);
    }

    private static void ConfigureHttpClientForRouter(IServiceProvider serviceProvider, HttpClient client)
    {
        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<RouterOptions>>();
        RouterHelper.ConfigureHttpClient(client, options.Value);
    }
}