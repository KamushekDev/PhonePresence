namespace PhonePresenceBot.Core.PhoneMonitoring;

public interface IPhonePresenceMonitoringService
{
    public void StartUpdatingInfo(CancellationToken token);
}