using System.Text.Json;
using Comandante;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PresenceBot.Core;
using PresenceBot.Core.Messages;
using PresenceBot.Infrastructure.Presence.Options;
using PresenceBot.Infrastructure.Telegram;
using PresenceBot.Infrastructure.VK.Options;
using PresenceBot.Services.Presence;
using VkNet;
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
                var options = serviceProvider
                    .CreateAsyncScope()
                    .ServiceProvider
                    .GetRequiredService<IOptionsSnapshot<VkOptions>>()
                    .Value;

                logger.LogInformation("Starting VK job");

                var api = new VkApi();

                await api.AuthorizeAsync(new ApiAuthParams()
                {
                    AccessToken = options.ApiKey
                }, stoppingToken);

                logger.LogInformation("VK API authorized");

                var server = api.Groups.GetLongPollServer(options.GroupId);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var poll = api.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams
                    {
                        Server = server.Server,
                        Key = server.Key,
                        Ts = server.Ts
                    });

                    server.Ts = poll.Ts;

                    foreach (var update in poll.Updates)
                    {
                        if (update.Type.Value == GroupUpdateType.MessageNew)
                        {
                            if (update.Instance is MessageNew message)
                                await HandleMessage(api, message, stoppingToken);
                        }
                    }

                    await Task.Delay(500, stoppingToken);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in vk job");
            }
        }
    }

    private async Task HandleMessage(
        VkApi api,
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
                    wasPresented => Reply(messageFormatter.GetInactiveClientMessage(wasPresented.ElapsedTime),
                        message.Message, api, token),
                    wasNeverPresented => Reply(
                        messageFormatter.GetNeverActiveClient(wasNeverPresented.ClientIdentity),
                        message.Message, api, token)
                );
            }
                break;

            default:
                await Reply("Я отвечаю только на нажатие кнопки \"Телефон дома?\"", message.Message, api, token);
                break;
        }
    }

    private async Task Reply(string text, Message message, VkApi api, CancellationToken token)
    {
        var action = new MessageKeyboardButtonAction()
        {
            Label = "Телефон дома?",
            Payload = JsonSerializer.Serialize(new VkMessagePayload(BotCommands.CheckPhoneCommand)),
            Type = KeyboardButtonActionType.Text
        };
        var keyboard = new KeyboardBuilder()
            .AddButton(action, KeyboardButtonColor.Primary)
            .Build();

        await api.Messages.SendAsync(new MessagesSendParams
        {
            PeerId = message.PeerId,
            RandomId = Environment.TickCount,
            Message = text,
            ReplyTo = message.Id,
            Keyboard = keyboard
        }, token);
    }
}

file record struct VkMessagePayload(string Command);