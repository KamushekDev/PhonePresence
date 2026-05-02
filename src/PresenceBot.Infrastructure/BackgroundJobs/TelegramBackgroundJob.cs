using System.Text.Json;
using Comandante;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PresenceBot.Core.Messages;
using PresenceBot.Core.Notifications;
using PresenceBot.Core.Notifications.Models;
using PresenceBot.Core.Telegram;
using PresenceBot.Infrastructure.Presence.Options;
using PresenceBot.Infrastructure.Telegram;
using PresenceBot.Services.Presence;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PresenceBot.Infrastructure.BackgroundJobs;

public class TelegramBackgroundJob(
    IServiceProvider serviceProvider,
    ILogger<TelegramBackgroundJob> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();     
                var client =  scope.ServiceProvider.GetRequiredService<IMyTelegramClient>();
                await client.StartAsync(stoppingToken);

                await client.ReceiveAsync(HandleUpdate, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in telegram job");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task HandleUpdate(IMyTelegramClient client, Update update, CancellationToken token)
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
                                    async wasPresented =>
                                    {
                                        await Reply(client, message,
                                            messageFormatter.GetInactiveClientMessage(wasPresented.ElapsedTime),
                                            token);
                                        await AddNotificationRequest(scope, message, wasPresented.ClientIdentity, token);
                                    },
                                    async wasNeverPresented =>
                                    {
                                        await Reply(client, message,
                                            messageFormatter.GetNeverActiveClient(wasNeverPresented.ClientIdentity),
                                            token);
                                        await AddNotificationRequest(scope, message, wasNeverPresented.ClientIdentity, token);
                                    });
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

    private async Task Reply(
        IMyTelegramClient client,
        Message originalMessage,
        string message,
        CancellationToken token)
    {
        await client.ReplyAsync(new TelegramReplyData(originalMessage.Chat.Id, originalMessage.MessageId), message, token);
    }                                                              

    private async Task AddNotificationRequest(AsyncServiceScope scope, Message message, string argClientIdentity,
        CancellationToken token)
    {
        var notificationRepository = scope.ServiceProvider.GetRequiredService<IPresenceNotificationsRepository>();
        
        await notificationRepository.AddRequest(new NotificationRequest()
        {
            ClientIdentity = argClientIdentity,
            Source = NotificationSource.Telegram,
            ReplyData = JsonSerializer.Serialize(new TelegramReplyData(message.Chat.Id, message.MessageId))
        }, token);
    }
}