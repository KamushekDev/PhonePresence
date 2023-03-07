using Telegram.Bot;
using Telegram.Bot.Types;

namespace PhonePresenceBot.Integration.Telegram.DependencyInjection;

public interface ITelegramUpdatePipelineBuilder
{
    public ITelegramUpdatePipelineBuilder AddHandler(
        Func<IServiceProvider, ITelegramBotClient, Update, CancellationToken, Task> handler);

    public ITelegramUpdatePipelineBuilder AddHandler<THandler>()
        where THandler : ITelegramUpdateHandler;

    public void Build();
}