using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhonePresenceBot.Core.PhonePresence;
using PhonePresenceBot.Infrastructure.Options;
using PhonePresenceBot.Infrastructure.Telegram.Filters;
using PhonePresenceBot.Integration.Telegram.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PhonePresenceBot.Infrastructure.Telegram.Handlers;

public class PhonePresenceHandler : ITelegramUpdateHandler
{
    private readonly ILogger<PhonePresenceHandler> _logger;
    private readonly OnlyPrivateMessagesFilter _onlyPrivateMessagesFilter;
    private readonly OldMessagesFilter _oldMessagesFilter;
    private readonly IPhonePresenceService _presenceService;
    private readonly Settings _settings;

    public PhonePresenceHandler(
        ILogger<PhonePresenceHandler> logger,
        OnlyPrivateMessagesFilter onlyPrivateMessagesFilter,
        OldMessagesFilter oldMessagesFilter,
        IOptionsSnapshot<Settings> settings,
        IPhonePresenceService presenceService)
    {
        _logger = logger;
        _onlyPrivateMessagesFilter = onlyPrivateMessagesFilter;
        _oldMessagesFilter = oldMessagesFilter;
        _presenceService = presenceService;
        _settings = settings.Value;
    }

    public async Task Handle(ITelegramBotClient client, Update update, CancellationToken token)
    {
        if (!_onlyPrivateMessagesFilter.ShouldHandle(update))
        {
            _logger.LogDebug("{HandlerType} skiped update with id {UpdateId} because of {FilterType}",
                this.GetType().Name, update.Id, _onlyPrivateMessagesFilter.GetType().Name);
            return;
        }

        if (!_oldMessagesFilter.ShouldHandle(update))
        {
            _logger.LogDebug("{HandlerType} skiped update with id {UpdateId} because of {FilterType}",
                this.GetType().Name, update.Id, _onlyPrivateMessagesFilter.GetType().Name);
            return;
        }
        
        var message = update.Message!;
        var senderId = message.From!.Id;
        if (!_settings.ProhibitedUserIds.Contains(senderId))
        {
            _logger.LogInformation(
                "{HandlerType} skips update with id {UpdateId} because user with id {UserId} isn't whitelisted",
                this.GetType().Name, update.Id, senderId
            );

            await client.SendTextMessageAsync(message.Chat.Id,
                "Ты не состоишь в списке людей, кто может использовать этого бота",
                replyToMessageId: message.MessageId,
                replyMarkup: new ReplyKeyboardRemove(), cancellationToken: token);

            return;
        }

        var request = new PhonePresenceRequest(PhoneName: _settings.PhoneName);
        var presenceResult = await _presenceService.IsPhonePresented(request, token);

        if (presenceResult)
        {
            await client.SendTextMessageAsync(
                message.Chat.Id,
                "Телефон подключен к WiFi",
                replyMarkup: new ReplyKeyboardMarkup(new[] { new KeyboardButton("Телефон дома?") }),
                replyToMessageId: message.MessageId,
                cancellationToken: token
            );
        }
        else
        {
            await client.SendTextMessageAsync(
                message.Chat.Id,
                "Телефон не подключен",
                replyMarkup: new ReplyKeyboardMarkup(new[] { new KeyboardButton("Телефон дома?") }),
                replyToMessageId: message.MessageId,
                cancellationToken: token
            );
        }
    }
}