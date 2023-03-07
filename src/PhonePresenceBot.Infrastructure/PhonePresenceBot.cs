using PhonePresenceBot.Core;
using PhonePresenceBot.Integration.Telegram;

namespace PhonePresenceBot.Infrastructure;

public class PhonePresenceBot : IPhonePresenceBot
{
    private readonly TelegramGateway _gateway;

    public PhonePresenceBot(TelegramGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task Run(CancellationToken token)
    {
        await _gateway.HandleRequests(token);
    }

    /*
     * Возможно стоит переписать так,
     * чтобы нужно было на объекте гейтвея вызывать настройку пайплайна
     * и вызывать что-то типо Work
     * 
     * А не в настройках DI, как это сделано сейчас
     * Мне кажется, что так будет намного понятнее
     */
}