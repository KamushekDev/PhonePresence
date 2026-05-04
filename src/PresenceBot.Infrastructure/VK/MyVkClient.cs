using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PresenceBot.Core.Vk;
using PresenceBot.Infrastructure.Telegram;
using PresenceBot.Infrastructure.VK.Options;
using VkNet;
using VkNet.Enums.StringEnums;
using VkNet.Model;

namespace PresenceBot.Infrastructure.VK;

public class MyVkClient(IServiceProvider serviceProvider, ILogger<MyVkClient> logger) : IMyVkClient
{
    private VkApi? _client;
    
    public async Task StartAsync(CancellationToken token)
    {
        if (_client is not null)
        {
            _client.Dispose();
        }
        
        var options = serviceProvider
            .CreateAsyncScope()
            .ServiceProvider
            .GetRequiredService<IOptionsSnapshot<VkOptions>>()
            .Value;

        logger.LogInformation("Starting VK job");

        var api = new VkApi();
        _client = api;

        await api.AuthorizeAsync(new ApiAuthParams()
        {
            AccessToken = options.ApiKey
        }, token);

        logger.LogInformation("VK API authorized");
    }

    public async Task<LongPollServerResponse?> GetLongPollServer(CancellationToken token)
    {
        if (_client is null)
            throw new Exception("Client is not initialized");  
        
        var options = serviceProvider
            .CreateAsyncScope()
            .ServiceProvider
            .GetRequiredService<IOptionsSnapshot<VkOptions>>()
            .Value;
        
        var server = await _client.Groups.GetLongPollServerAsync(options.GroupId, token);

        return server;
    }

    public async Task<BotsLongPollHistoryResponse> GetUpdates(LongPollServerResponse server, CancellationToken token)
    {
        if (_client is null)
            throw new Exception("Client is not initialized"); 
        
        var poll = _client.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams
        {
            Server = server.Server,
            Key = server.Key,
            Ts = server.Ts
        });

        return poll;
    }

    public async Task Reply(VkReplyData replyData, string text, CancellationToken token)
    {
        if (_client is null)
            throw new Exception("Client is not initialized"); 
        
        var action = new MessageKeyboardButtonAction()
        {
            Label = "Телефон дома?",
            Payload = JsonSerializer.Serialize(new VkMessagePayload(BotCommands.CheckPhoneCommand)),
            Type = KeyboardButtonActionType.Text
        };
        var keyboard = new KeyboardBuilder()
            .AddButton(action, KeyboardButtonColor.Primary)
            .Build();

        await _client.Messages.SendAsync(new MessagesSendParams
        {
            PeerId = replyData.PeerId,
            RandomId = Environment.TickCount,
            Message = text,
            ReplyTo = replyData.MessageId,
            Keyboard = keyboard
        }, token); 
    }
}