using Comandante;
using Microsoft.Extensions.Options;
using PresenceBot.Core.Messages;
using PresenceBot.Core.Notifications;
using PresenceBot.Core.Notifications.Models;
using PresenceBot.Core.Presence;
using PresenceBot.Infrastructure.Presence.Options;

namespace PresenceBot.Infrastructure.Presence;

public class PhonePresenceHandler(
    IClientPresenceService service,
    IMessageFormatter messageFormatter,
    IPresenceNotificationsRepository notificationsRepository,
    IOptionsSnapshot<PresenceOptions> optionsSnapshot)
    : ICommandHandler<PhonePresenceHandler.Command, PhonePresenceHandler.Response>
{
    /*
     * Сделать готовый результат
     * Сделать интеграцию с календарём
     * Сделать локальный телеграм сервер
     * Переключить
     */

    public async Task<Response> Handle(Command command, CancellationToken token)
    {
        // todo: filter by userid
        var options = optionsSnapshot.Value;

        var presence =
            await service.GetIdentityPresence(options.WantedClientIdentity, options.WantedConfidenceInterval, token);

        var response = presence.Match(
            presented => messageFormatter.GetActiveClientMessage(),
            wasPresented => messageFormatter.GetInactiveClientMessage(wasPresented.ElapsedTime),
            neverPresented => messageFormatter.GetNeverActiveClient(neverPresented.ClientIdentity));

        var notificationCreated = await presence.Match(
            online => Task.FromResult(false),
            async was => await AddNotificationRequest(was.ClientIdentity, command.Source, command.ReplyData, token),
            async never => await AddNotificationRequest(never.ClientIdentity, command.Source, command.ReplyData, token)
        );

        return new Response(response, notificationCreated);
    }

    public record Command(NotificationSource Source, string ReplyData) : ICommand<Response>;

    public record Response(string ResponseToUser, bool CreatedDelayedNotification);

    private async Task<bool> AddNotificationRequest(string clientIdentity, NotificationSource source, string replyData,
        CancellationToken token)
    {
        await notificationsRepository.AddRequest(new NotificationRequest()
        {
            ClientIdentity = clientIdentity,
            Source = source,
            ReplyData = replyData
        }, token);

        return true;
    }
}