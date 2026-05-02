using System.Text.Json;
using Comandante;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PresenceBot.Core.Messages;
using PresenceBot.Core.Notifications;
using PresenceBot.Core.Notifications.Models;
using PresenceBot.Core.Vk;
using PresenceBot.Infrastructure.Presence.Options;
using PresenceBot.Infrastructure.Telegram;
using PresenceBot.Services.Presence;
using VkNet.Enums.StringEnums;
using VkNet.Model;

namespace PresenceBot.Infrastructure.BackgroundJobs;

public class VkBackgroundJob(
    IServiceProvider serviceProvider,
    ILogger<TelegramBackgroundJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var client = scope.ServiceProvider.GetRequiredService<IMyVkClient>();
                var server = await client.StartAsync(stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var poll = await client.GetUpdates(server,  stoppingToken);

                    server.Ts = poll.Ts;

                    foreach (var update in poll.Updates)
                    {
                        if (update.Type.Value == GroupUpdateType.MessageNew)
                        {
                            if (update.Instance is MessageNew message)
                                await HandleMessage(client, message, stoppingToken);
                        }
                    }

                    await Task.Delay(50, stoppingToken);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in vk job");
            }
        }
    }

    private async Task HandleMessage(
        IMyVkClient api,
        MessageNew message,
        CancellationToken token)
    {
        var payload = message.Message.Payload is { } payloadText
            ? JsonSerializer.Deserialize<VkMessagePayload>(payloadText)
            : new VkMessagePayload("Unknown");

        switch (payload.Command)
        {
            case BotCommands.CheckPhoneCommand:
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var queryDispatcher = scope.ServiceProvider
                    .GetRequiredService<IQueryDispatcher>();

                var presenceOptions = scope.ServiceProvider
                    .GetRequiredService<IOptionsSnapshot<PresenceOptions>>()
                    .Value;

                var request = new PhonePresenceHandler.Query()
                {
                    ClientIdentity = presenceOptions.WantedClientIdentity,
                    ConfidenceInterval = presenceOptions.WantedConfidenceInterval
                };
                var result = await queryDispatcher.Dispatch(request, token);

                var messageFormatter = scope.ServiceProvider
                    .GetRequiredService<IMessageFormatter>();

                await result.Match(
                    presented => Reply(messageFormatter.GetActiveClientMessage(), message.Message, api, token),
                    async wasPresented =>
                    {
                        await Reply(messageFormatter.GetInactiveClientMessage(wasPresented.ElapsedTime),
                            message.Message, api, token);
                        await AddNotificationRequest(scope, message.Message, wasPresented.ClientIdentity, token);
                    },
                    async wasNeverPresented =>
                    {
                        await Reply(
                            messageFormatter.GetNeverActiveClient(wasNeverPresented.ClientIdentity),
                            message.Message, api, token);
                        await AddNotificationRequest(scope, message.Message, wasNeverPresented.ClientIdentity, token);
                    });
            }
                break;

            default:
                await Reply("Я отвечаю только на нажатие кнопки \"Телефон дома?\"", message.Message, api, token);
                break;
        }
    }

    private async Task Reply(string text, Message message, IMyVkClient api, CancellationToken token)
    {
        await api.Reply(new VkReplyData(message.PeerId.Value, message.Id.Value), text, token);
    }
    
    private async Task AddNotificationRequest(AsyncServiceScope scope, Message message, string argClientIdentity,
        CancellationToken token)
    {
        var notificationRepository = scope.ServiceProvider.GetRequiredService<IPresenceNotificationsRepository>();
        
        await notificationRepository.AddRequest(new NotificationRequest()
        {
            ClientIdentity = argClientIdentity,
            Source = NotificationSource.Vk,
            ReplyData = JsonSerializer.Serialize(new VkReplyData(message.PeerId.Value, message.Id.Value))
        }, token);
    }
}

