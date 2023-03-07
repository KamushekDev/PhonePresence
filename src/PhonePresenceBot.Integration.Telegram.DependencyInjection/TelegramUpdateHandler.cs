using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using OneOf;

namespace PhonePresenceBot.Integration.Telegram.DependencyInjection;

public class TelegramUpdateHandler : Telegram.ITelegramUpdateHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IList<OneOf<Type, Func<IServiceProvider, ITelegramBotClient, Update, CancellationToken, Task>>> _handlers;
    private readonly ILogger<TelegramUpdateHandler> _logger;

    public TelegramUpdateHandler(
        IServiceProvider serviceProvider,
        IList<OneOf<Type, Func<IServiceProvider, ITelegramBotClient, Update, CancellationToken, Task>>> handlers,
        ILogger<TelegramUpdateHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _handlers = handlers;
        _logger = logger;
    }

    public async Task Handle(ITelegramBotClient client, Update update, CancellationToken token)
    {
        foreach (var handler in _handlers)
        {
            try
            {
                await handler.Match(
                    type =>
                    {
                        var h = (ITelegramUpdateHandler)_serviceProvider.GetRequiredService(type);
                        return h.Handle(client, update, token);
                    },
                    func => func(_serviceProvider, client, update, token)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Got exception during handler execution");
            }
        }
    }
}