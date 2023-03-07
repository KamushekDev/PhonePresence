using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using OneOf;
using Telegram.Bot;

namespace PhonePresenceBot.Integration.Telegram.DependencyInjection;

public class TelegramUpdatePipelineBuilder : ITelegramUpdatePipelineBuilder
{
    private readonly IServiceCollection _serviceCollection;

    private readonly List<OneOf<Type, Func<IServiceProvider, ITelegramBotClient, Update, CancellationToken, Task>>>
        _handlers = new();

    public TelegramUpdatePipelineBuilder(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public ITelegramUpdatePipelineBuilder AddHandler(
        Func<IServiceProvider, ITelegramBotClient, Update, CancellationToken, Task> handler)
    {
        _handlers.Add(handler);
        return this;
    }

    public ITelegramUpdatePipelineBuilder AddHandler<THandler>()
        where THandler : ITelegramUpdateHandler
    {
        _handlers.Add(typeof(THandler));
        return this;
    }

    public void Build()
    {
        _serviceCollection.AddSingleton<Telegram.ITelegramUpdateHandler, TelegramUpdateHandler>(
            ResolveFromServiceProvider);
    }

    private TelegramUpdateHandler ResolveFromServiceProvider(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<TelegramUpdateHandler>>();

        return new TelegramUpdateHandler(serviceProvider, _handlers, logger);
    }
}