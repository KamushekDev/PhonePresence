namespace PhonePresenceBot.Infrastructure.Telegram;

public record BaseCommand(long UserId, int MessageId, long ChatId);

public record StartCommand(long UserId, int MessageId, long ChatId)
    : BaseCommand(UserId, MessageId, ChatId);

public record CheckPhonePresenceCommand(long UserId, int MessageId, long ChatId)
    : BaseCommand(UserId, MessageId, ChatId);