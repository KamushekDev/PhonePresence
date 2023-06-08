using Telegram.Bot.Types;

namespace PresenceBot.Infrastructure.Telegram
{
    public static class BotCommands
    {
        public const string CheckPhoneCommand = "/phone";

        public static BotCommand CheckPhone => new()
        {
            Command = CheckPhoneCommand,
            Description = "Проверить подключен ли телефон к домашней сети"
        };
    }
}