using System.Text.Json;
using Comandante;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PresenceBot.Core.Notifications.Models;
using PresenceBot.Core.Vk;
using PresenceBot.Infrastructure.Presence;
using PresenceBot.Infrastructure.Telegram;
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
                await client.StartAsync(stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var server = await client.GetLongPollServer(stoppingToken);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var poll = await client.GetUpdates(server!, stoppingToken);

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
                        catch
                        {
                            break;
                        }
                    }
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
                var commandDispatcher = scope.ServiceProvider
                    .GetRequiredService<ICommandDispatcher>();

                var request = new PhonePresenceHandler.Command(
                    NotificationSource.Vk,
                    JsonSerializer.Serialize(new VkReplyData(message.Message.PeerId.Value, message.Message.Id.Value))
                );
                var result = await commandDispatcher.Dispatch(request, token);

                await Reply(result.ResponseToUser, message.Message, api, token);
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
}