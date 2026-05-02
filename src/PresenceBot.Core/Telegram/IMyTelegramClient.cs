using Telegram.Bot.Types;

namespace PresenceBot.Core.Telegram;

public interface IMyTelegramClient
{
    Task StartAsync(CancellationToken token);
    Task ReceiveAsync(Func<IMyTelegramClient, Update, CancellationToken, Task> handleUpdate, CancellationToken token);
    Task ReplyAsync(TelegramReplyData replyData, string text, CancellationToken token);
}