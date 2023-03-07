using Microsoft.Extensions.Logging;
using PhonePresenceBot.Integration.Telegram.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PhonePresenceBot.Integration.Telegram;

public class TelegramGateway
{
    private readonly ILogger<TelegramGateway> _logger;
    private readonly ITelegramUpdateHandler _handler;
    private readonly TelegramBotClient _client;

    public TelegramGateway(TelegramOptions options,
        ITelegramUpdateHandler handler,
        ILogger<TelegramGateway> logger)
    {
        _logger = logger;
        _handler = handler;
        _client = new TelegramBotClient(options.ApiKey);
    }

    public async Task SetupCommands(BotCommand[] commands, CancellationToken token)
    {
        await _client.SetMyCommandsAsync(new[]
        {
            new BotCommand() { Command = "/phone", Description = "Проверить подключен ли телефон к домашней сети" },
        }, cancellationToken: token);
    }

    public async Task HandleRequests(
        CancellationToken token
    )
    {
        await _client.ReceiveAsync(
            (client, update, innerToken) => _handler.Handle(client, update, innerToken),
            HandlePollError,
            cancellationToken: token
        );
    }

    private Task HandlePollError(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        _logger.LogError(exception, "Poll error occured");
        return Task.CompletedTask;
    }
}