using System.Globalization;
using Humanizer;
using PresenceBot.Core.Messages;

namespace PresenceBot.Infrastructure.Messages;

public class MessageFormatter : IMessageFormatter
{
    private readonly static CultureInfo CultureInfo = new CultureInfo("ru-RU");
    private const int HumanizerPrecision = 4;

    public string GetActiveClientMessage()
    {
        return "Клиент в сети :)";
    }

    public string GetInactiveClientMessage(TimeSpan inactivityTimeSpan)
    {
        var humanizedTimeSpan = inactivityTimeSpan.Humanize(
            precision: HumanizerPrecision,
            minUnit: TimeUnit.Minute,
            culture: CultureInfo);

        return $"Клиент был в сети {humanizedTimeSpan} назад. Я пришлю уведомление, когда он появится в сети!";
    }

    public string GetNeverActiveClient(string clientIdentity)
    {
        return
            $"К сети никогда не был подключён клиент с идентификатором `{clientIdentity}`. Я пришлю уведомление, когда он появится в сети!";
    }

    public string GetBecameActiveNotification(DateTimeOffset lastActiveTimestamp, TimeSpan inactivityTimeSpan)
    {
        var timestamp = lastActiveTimestamp.Humanize(culture: CultureInfo);
        var span = inactivityTimeSpan.Humanize(precision: HumanizerPrecision, minUnit: TimeUnit.Minute,
            culture: CultureInfo);
        return $"Клиент появился в сети {timestamp}. Вне сети клиент находился {span}.";
    }

    public string GetFirstAppearanceNotification(string clientIdentity)
    {
        return $"Клиент `{clientIdentity}` впервые появился в сети!";
    }
}