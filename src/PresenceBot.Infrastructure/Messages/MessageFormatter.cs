using System.Globalization;
using PresenceBot.Core.Messages;

namespace PresenceBot.Infrastructure.Messages;

public class MessageFormatter : IMessageFormatter
{
    public string GetActiveClientMessage()
    {
        return "Клиент в сети :)";
    }

    public string GetInactiveClientMessage(TimeSpan inactivityTimeSpan)
    {
        return $"Клиент был в сети {Humanizer.TimeSpanHumanizeExtensions.Humanize(inactivityTimeSpan, culture: CultureInfo.CreateSpecificCulture("ru-ru"))} назад";
    }

    public string GetNeverActiveClient(string clientIdentity)
    {
        return $"К сети никогда не был подключён клиент с идентификатором `{clientIdentity}`";
    }
}