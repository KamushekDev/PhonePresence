using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace PresenceBot.Infrastructure.Telegram;

public class TelegramService
{
    public async Task SendPresenceNotification(ITelegramBotClient client, long chatId, CancellationToken token)
    {
        await client.SendTextMessageAsync(
            chatId,
            $"Телефон подключился к сети",
            replyMarkup: new ReplyKeyboardMarkup(new[] { new KeyboardButton("Телефон дома?") }),
            cancellationToken: token
        );
    }
}