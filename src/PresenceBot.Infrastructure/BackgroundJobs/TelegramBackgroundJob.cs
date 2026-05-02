using Comandante;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PresenceBot.Core;
using PresenceBot.Core.Messages;
using PresenceBot.Infrastructure.Presence.Options;
using PresenceBot.Infrastructure.Telegram;
using PresenceBot.Infrastructure.Telegram.Options;
using PresenceBot.Services.Presence;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PresenceBot.Infrastructure.BackgroundJobs;

public class TelegramBackgroundJob(
    IServiceProvider serviceProvider,
    IHttpClientFactory httpClientFactory,
    ILogger<TelegramBackgroundJob> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var options = serviceProvider
                    .CreateAsyncScope()
                    .ServiceProvider
                    .GetRequiredService<IOptionsSnapshot<TelegramOptions>>()
                    .Value;

                var httpClient = httpClientFactory.CreateClient("WithProxy");

                var client = new TelegramBotClient(options.ApiKey, httpClient);

                logger.LogInformation("Starting telegram job");
                var isValidToken = await client.TestApi(stoppingToken);
                if (!isValidToken)
                    throw new Exception("Invalid token");
                logger.LogInformation("Telegram token is validated");

                await SetupCommands(client, stoppingToken);
                logger.LogInformation("Telegram set up commands");
                await client.ReceiveAsync(HandleUpdate, HandlePollError, receiverOptions: new ReceiverOptions(),
                    cancellationToken: stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in telegram job");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task HandleUpdate(ITelegramBotClient client, Update update, CancellationToken token)
    {
        logger.LogDebug("Received update with id: {UpdateId}", update.Id);
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
                    {
                        await using var scope = serviceProvider.CreateAsyncScope();
                        var queryDispatcher = scope
                            .ServiceProvider
                            .GetRequiredService<IQueryDispatcher>();

                        var presenceOptions = scope.ServiceProvider
                            .GetRequiredService<IOptionsSnapshot<PresenceOptions>>()
                            .Value;

                        switch (message.EntityValues!.First())
                        {
                            case BotCommands.CheckPhoneCommand:
                                var request = new PhonePresenceHandler.Query()
                                {
                                    ClientIdentity = presenceOptions.WantedClientIdentity,
                                    ConfidenceInterval = presenceOptions.WantedConfidenceInterval
                                };
                                var result = await queryDispatcher.Dispatch(request, token);

                                var messageFormatter = scope.ServiceProvider.GetRequiredService<IMessageFormatter>();
                                
                                await result.Match(
                                    presented => Reply(client, message, messageFormatter.GetActiveClientMessage(), token),
                                    wasPresented => Reply(client, message,
                                       messageFormatter.GetInactiveClientMessage(wasPresented.ElapsedTime),
                                        token),
                                    wasNeverPresented => Reply(client, message,
                                        messageFormatter.GetNeverActiveClient(wasNeverPresented.ClientIdentity),
                                        token)
                                );
                                break;
                            default:
                                await Reply(client, message, "Я понимаю только заготовленные команды :(", token);
                                return;
                        }

                        break;
                    }
                    default:
                        await Reply(client, message, "Я понимаю только текстовые сообщения :(", token);
                        return;
                }

                break;
            default:
                logger.LogDebug("Skipped update with id {UpdateId} because of type {UpdateType}", update.Id,
                    update.Type);
                return;
        }
    }

    private Task HandlePollError(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        logger.LogError(exception, "Poll error occured");
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

        logger.LogDebug("Sent message {@Message}", sentMessage);
    }
}