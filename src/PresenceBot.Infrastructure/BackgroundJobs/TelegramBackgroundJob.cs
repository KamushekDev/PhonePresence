using Comandante;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PresenceBot.Core;
using PresenceBot.Infrastructure.Telegram;
using PresenceBot.Infrastructure.Telegram.Options;
using PresenceBot.Services.Presence;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PresenceBot.Infrastructure.BackgroundJobs;

public class TelegramBackgroundJob : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TelegramBackgroundJob> _logger;

    private CancellationTokenSource? _cts;
    private Task? _worker;

    public TelegramBackgroundJob(
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory,
        ILogger<TelegramBackgroundJob> logger)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
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

        var httpClient = _httpClientFactory.CreateClient("WithProxy");

        var client = new TelegramBotClient(options.ApiKey, httpClient);

        _logger.LogInformation("Starting telegram job");
        var isValidToken = await client.TestApi(_cts.Token);
        if (!isValidToken)
            throw new Exception("Invalid token");
        _logger.LogInformation("Telegram token is validated");
        
        await SetupCommands(client, _cts.Token);
        _logger.LogInformation("Telegram set up commands");
        _worker = client.ReceiveAsync(HandleUpdate, HandlePollError, receiverOptions: new ReceiverOptions(){},cancellationToken: _cts.Token);
        _logger.LogInformation("Telegram job is started successfully");
    }

    public Task StopAsync(CancellationToken token)
    {
        _cts?.Cancel();

        return _worker ?? Task.CompletedTask;
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
                                var request = new PhonePresenceHandler.Query() { ClientIdentity = MyConstants.PhoneName, ConfidenceInterval = MyConstants.ConfidenceInterval};
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
        await client.SetMyCommands([
            BotCommands.CheckPhone
        ], cancellationToken: token);
    }

    private async Task Reply(
        ITelegramBotClient client,
        Message originalMessage,
        string message,
        CancellationToken token)
    {
        var sentMessage = await client.SendMessage(
            originalMessage.Chat.Id,
            message,
            replyParameters: new ReplyParameters()
            {
                MessageId = originalMessage.MessageId,
            },
            replyMarkup: new ReplyKeyboardMarkup(
                new KeyboardButton(BotCommands.CheckPhoneCommand)
            ),
            // new ForceReplyMarkup() { InputFieldPlaceholder = "Телефон дома?" },
            cancellationToken: token);
        
        _logger.LogInformation("Sent message {@Message}", sentMessage);
    }
}