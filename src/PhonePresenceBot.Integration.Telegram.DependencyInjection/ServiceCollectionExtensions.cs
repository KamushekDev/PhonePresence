using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhonePresenceBot.Integration.Telegram.Options;

namespace PhonePresenceBot.Integration.Telegram.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static ITelegramUpdatePipelineBuilder AddTelegramPipeline(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddOptions<TelegramOptions>()
            .BindConfiguration(TelegramOptions.SectionName);

        serviceCollection.AddSingleton<TelegramGateway>(ResolveFromProvider);

        var pipelineBuilder = new TelegramUpdatePipelineBuilder(serviceCollection);
        return pipelineBuilder;
    }

    private static TelegramGateway ResolveFromProvider(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<TelegramOptions>>();
        var handler = serviceProvider.GetRequiredService<Telegram.ITelegramUpdateHandler>();
        var logger = serviceProvider.GetRequiredService<ILogger<TelegramGateway>>();

        return new TelegramGateway(options.Value, handler, logger);
    }
}