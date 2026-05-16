using System.Text.Json;
using Comandante;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PresenceBot.Core.Notifications.Models;
using PresenceBot.Core.Telegram;
using PresenceBot.Infrastructure.Presence;
using PresenceBot.Infrastructure.Telegram;
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
                var client = scope.ServiceProvider.GetRequiredService<IMyTelegramClient>();
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
                        var commandDispatcher = scope
                            .ServiceProvider
                            .GetRequiredService<ICommandDispatcher>();

                        switch (message.EntityValues!.First())
                        {
                            case BotCommands.CheckPhoneCommand:
                                var request = new PhonePresenceHandler.Command(
                                    NotificationSource.Telegram,
                                    JsonSerializer.Serialize(new TelegramReplyData(message.Chat.Id, message.MessageId))
                                );
                                var result = await commandDispatcher.Dispatch(request, token);

                                await Reply(client, message, result.ResponseToUser, token);
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
        await client.ReplyAsync(new TelegramReplyData(originalMessage.Chat.Id, originalMessage.MessageId), message,
            token);
    }
}