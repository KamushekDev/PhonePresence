using VkNet.Model;

namespace PresenceBot.Core.Vk;

public interface IMyVkClient
{
    Task<LongPollServerResponse?> StartAsync(CancellationToken token);
    Task<BotsLongPollHistoryResponse> GetUpdates(LongPollServerResponse server, CancellationToken token);
    Task Reply(VkReplyData replyData, string text, CancellationToken token);
}