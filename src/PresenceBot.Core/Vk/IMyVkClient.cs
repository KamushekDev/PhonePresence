using VkNet.Model;

namespace PresenceBot.Core.Vk;

public interface IMyVkClient
{
    Task StartAsync(CancellationToken token); 
    Task<LongPollServerResponse?> GetLongPollServer(CancellationToken token);
    Task<BotsLongPollHistoryResponse> GetUpdates(LongPollServerResponse server, CancellationToken token);
    Task Reply(VkReplyData replyData, string text, CancellationToken token);
}