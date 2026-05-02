using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PresenceBot.Core.Telegram;
using PresenceBot.Infrastructure.Telegram.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PresenceBot.Infrastructure.Telegram;

public class MyTelegramClient(
    IHttpClientFactory httpClientFactory,
    IServiceProvider serviceProvider,
    ILogger<MyTelegramClient> logger
) : IMyTelegramClient
{
    private TelegramBotClient? _botClient;

    public async Task StartAsync(CancellationToken token)
    {
        if (_botClient is not null)
        {
            await _botClient.Close(token);
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<TelegramOptions>>();
        
        var httpClient = httpClientFactory.CreateClient("WithProxy");

        var client = new TelegramBotClient(options.Value.ApiKey, httpClient);
        _botClient = client;

        logger.LogInformation("Starting telegram job");
        var isValidToken = await client.TestApi(token);
        if (!isValidToken)
            throw new Exception("Invalid token");
        logger.LogInformation("Telegram token is validated");

        await SetupCommands(client, token);
    }

    public async Task ReceiveAsync(Func<IMyTelegramClient, Update, CancellationToken, Task> handleUpdate, CancellationToken token)
    {
        if (_botClient == null)
            throw new Exception("Telegram bot client not initialized");
        await _botClient.ReceiveAsync((_, update, innerToken) => handleUpdate(this, update, innerToken), HandlePollError, receiverOptions: new ReceiverOptions(),
            cancellationToken: token);
    }

    public async Task ReplyAsync(TelegramReplyData replyData, string text, CancellationToken token)
    {
        if (_botClient == null)
            throw new Exception("Telegram bot client not initialized");
        
        var sentMessage = await _botClient.SendMessage(
            replyData.ChatId,
            text,
            replyParameters: new ReplyParameters()
            {
                MessageId = replyData.MessageId,
            },
            replyMarkup: new ReplyKeyboardMarkup(
                new KeyboardButton(BotCommands.CheckPhoneCommand)
            ),
            // new ForceReplyMarkup() { InputFieldPlaceholder = "Телефон дома?" },
            cancellationToken: token);

        logger.LogDebug("Sent message {@Message}", sentMessage);
    }

    private async Task SetupCommands(ITelegramBotClient client, CancellationToken token)
    {
        await client.SetMyCommands([
            BotCommands.CheckPhone
        ], cancellationToken: token);
    }

    private Task HandlePollError(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        logger.LogError(exception, "Poll error occured");
        return Task.CompletedTask;
    }
}