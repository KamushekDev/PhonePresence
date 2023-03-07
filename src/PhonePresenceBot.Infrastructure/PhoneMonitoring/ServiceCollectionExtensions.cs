using Microsoft.Extensions.DependencyInjection;
using PhonePresenceBot.Core.PhoneMonitoring;

namespace PhonePresenceBot.Infrastructure.PhoneMonitoring;

public static class ServiceCollectionExtensions
{
    public static void AddPhoneMonitoring(this IServiceCollection services)
    {
        services.AddSingleton<IPhonePresenceMonitoringService, PhonePresenceMonitoringService>();
        
        services.AddSingleton<IPhonePresenceInfoRepository, PhonePresenceInfoRepository>();
    }
}