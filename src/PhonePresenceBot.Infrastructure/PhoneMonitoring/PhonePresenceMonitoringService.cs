using PhonePresenceBot.Core.PhoneMonitoring;
using PhonePresenceBot.Core.PhonePresence;

namespace PhonePresenceBot.Infrastructure.PhoneMonitoring;

public class PhonePresenceMonitoringService : IPhonePresenceMonitoringService
{
    private readonly IPhonePresenceService _phonePresenceService;

    private PeriodicTimer? _timer;
    private Task? _timerTask;

    public PhonePresenceMonitoringService(
        IPhonePresenceService phonePresenceService,
        IPhonePresenceInfoRepository repository
    )
    {
        _phonePresenceService = phonePresenceService;
    }

    public void StartUpdatingInfo(CancellationToken token)
    {
        _timer = new PeriodicTimer(TimeSpan.FromMinutes(1))
    }

    private
}