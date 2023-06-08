using Comandante;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PresenceBot.Infrastructure.Telegram;
using PresenceBot.Infrastructure.Telegram.Options;
using PresenceBot.Services.Presence;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PresenceBot.Infrastructure.BackgroundJobs;

public class TelegramBackgroundJob : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramBackgroundJob> _logger;

    private CancellationTokenSource? _cts;
    private Task? _worker;

    public TelegramBackgroundJob(
        IServiceProvider serviceProvider,
        ILogger<TelegramBackgroundJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken startToken)
    {
        _cts = new CancellationTokenSource();

        var options = _serviceProvider
            .CreateAsyncScope()
            .ServiceProvider
            .GetRequiredService<IOptionsSnapshot<TelegramOptions>>()
            .Value;

        var client = new TelegramBotClient(options.ApiKey);
        await SetupCommands(client, startToken);
        _worker = client.ReceiveAsync(HandleUpdate, HandlePollError, cancellationToken: _cts.Token);
    }

    public Task StopAsync(CancellationToken token)
    {
        _cts?.Cancel();

        return Task.CompletedTask;
    }

    private async Task HandleUpdate(ITelegramBotClient client, Update update, CancellationToken token)
    {
        _logger.LogDebug("Received update with id: {UpdateId}", update.Id);
        switch (update.Type)
        {
            case UpdateType.Message:
                var message = update.Message!;
                var fromId = message.From?.Id;

                if (fromId is null)
                {
                    await Reply(client, message, "Не знаю как так вышло, но я не знаю кто ты.", token);
                    return;
                }

                switch (message.Type)
                {
                    case MessageType.Text when
                        message.Entities is not null
                        && message.Entities.Length == 1
                        && message.Entities.All(x => x.Type is MessageEntityType.BotCommand):

                        var queryDispatcher = _serviceProvider
                            .CreateAsyncScope()
                            .ServiceProvider
                            .GetRequiredService<IQueryDispatcher>();

                        switch (message.EntityValues!.First())
                        {
                            case BotCommands.CheckPhoneCommand:
                                var request = new PhonePresenceHandler.Query() { FromUserId = fromId.Value };
                                var result = await queryDispatcher.Dispatch(request, token);

                                await result.Match(
                                    presented => Reply(client, message, "Клиент сейчас в сети :)", token),
                                    wasPresented => Reply(client, message,
                                        $"Клиент был в сети {(int)wasPresented.ElapsedTime.TotalMinutes} минут назад",
                                        token),
                                    wasNeverPresented => Reply(client, message,
                                        $"К сети никогда не был подключён клиент с идентификатором {wasNeverPresented.ClientIdentity}",
                                        token)
                                );
                                break;
                            default:
                                await Reply(client, message, "Я понимаю только заготовленные команды :(", token);
                                return;
                        }

                        break;
                    default:
                        await Reply(client, message, "Я понимаю только текстовые сообщения :(", token);
                        return;
                }

                break;
            default:
                _logger.LogDebug("Skipped update with id {UpdateId} because of type {UpdateType}", update.Id,
                    update.Type);
                return;
        }
    }

    private Task HandlePollError(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        _logger.LogError(exception, "Poll error occured");
        return Task.CompletedTask;
    }

    private async Task SetupCommands(ITelegramBotClient client, CancellationToken token)
    {
        await client.SetMyCommandsAsync(new[]
        {
            BotCommands.CheckPhone,
        }, cancellationToken: token);
    }

    private async Task Reply(
        ITelegramBotClient client,
        Message originalMessage,
        string message,
        CancellationToken token)
    {
        await client.SendTextMessageAsync(
            originalMessage.Chat.Id,
            message,
            replyToMessageId: originalMessage.MessageId,
            replyMarkup:
            new ReplyKeyboardMarkup(new[] { new KeyboardButton("/phone"), new KeyboardButton("Телефон дома?") }),
            // new ForceReplyMarkup() { InputFieldPlaceholder = "Телефон дома?" },
            cancellationToken: token);
    }
}