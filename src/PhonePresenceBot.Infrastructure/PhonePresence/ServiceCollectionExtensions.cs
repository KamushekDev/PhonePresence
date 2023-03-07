using Microsoft.Extensions.DependencyInjection;
using PhonePresenceBot.Core.PhonePresence;
using PhonePresenceBot.Infrastructure.Options;

namespace PhonePresenceBot.Infrastructure.PhonePresence;

public static class ServiceCollectionExtensions
{
    public static void AddPhonePresence(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<Settings>()
            .BindConfiguration(Settings.SectionName);

        serviceCollection.AddTransient<IPhonePresenceService, PhonePresenceService>();
    }
}