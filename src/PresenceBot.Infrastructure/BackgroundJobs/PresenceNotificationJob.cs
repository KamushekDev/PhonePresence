using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PresenceBot.Core.MessageBus;
using PresenceBot.Core.Messages;
using PresenceBot.Core.Notifications;
using PresenceBot.Core.Notifications.Models;
using PresenceBot.Core.Presence.Models;
using PresenceBot.Core.Telegram;
using PresenceBot.Core.Vk;

namespace PresenceBot.Infrastructure.BackgroundJobs;

public class PresenceNotificationJob(
    IMessageBus<PresenceNotification> notificationsBus,
    IServiceProvider provider,
    ILogger<PresenceNotificationJob> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = provider.CreateAsyncScope();
                var repository = scope.ServiceProvider.GetRequiredService<IPresenceNotificationsRepository>();
                var notification = await notificationsBus.Consume(stoppingToken);
                var requests = await repository.GetRequests(stoppingToken);


                var clientIdentity = notification.Match(x => x.ClientIdentity, x => x.ClientIdentity);

                var suitableRequests = requests.Where(x => x.ClientIdentity == clientIdentity).ToList();
                if (suitableRequests.Count == 0)
                    continue;

                var messageFormatter = scope.ServiceProvider.GetRequiredService<IMessageFormatter>();

                var telegramClient = scope.ServiceProvider.GetRequiredService<IMyTelegramClient>();
                var vkClient = scope.ServiceProvider.GetRequiredService<IMyVkClient>();

                var text = notification.Match(x => messageFormatter.GetBecameActiveNotification(x.BecameActiveAt, x.InactivityPeriod), x => messageFormatter.GetFirstAppearanceNotification(x.ClientIdentity));

                foreach (var request in suitableRequests)
                {
                    switch (request.Source)
                    {
                        case NotificationSource.Vk:
                            await vkClient.Reply(
                                JsonSerializer.Deserialize<VkReplyData>(request.ReplyData)!, text, stoppingToken);
                            break;
                        case NotificationSource.Telegram:
                            await telegramClient.ReplyAsync(
                                JsonSerializer.Deserialize<TelegramReplyData>(request.ReplyData)!, text, stoppingToken);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                
                await repository.RemoveRequests(suitableRequests, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in notifications job");
            }
        }
    }
}