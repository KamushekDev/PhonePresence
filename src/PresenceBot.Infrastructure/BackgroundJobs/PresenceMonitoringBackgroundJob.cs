using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PresenceBot.Core.MessageBus;
using PresenceBot.Core.Presence;
using PresenceBot.Core.Presence.Models;
using PresenceBot.Integration.Router.Exceptions;

namespace PresenceBot.Infrastructure.BackgroundJobs;

public class PresenceMonitoringBackgroundJob : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBus<PresenceInfo> _messageBus;
    private readonly ILogger<PresenceMonitoringBackgroundJob> _logger;

    private Task? _worker;
    private CancellationTokenSource? _cts;
    private PeriodicTimer? _timer;

    public PresenceMonitoringBackgroundJob(IServiceProvider serviceProvider,
        IMessageBus<PresenceInfo> messageBus,
        ILogger<PresenceMonitoringBackgroundJob> logger)
    {
        _serviceProvider = serviceProvider;
        _messageBus = messageBus;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new TaskFactory();
        _cts = new CancellationTokenSource();
        _worker = factory.StartNew(
            () => Worker(_cts.Token),
            _cts.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        _timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts!.Cancel();
        await _worker!;
    }

    private async Task Worker(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await MonitorClients(token);
                await _timer!.WaitForNextTickAsync(token);
            }
            catch (RouterLoginFailed ex)
            {
                _logger.LogError(ex, "Failed login, while getting client presence infos");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong on getting client presence infos");
            }
        }
    }

    private async Task MonitorClients(CancellationToken token)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var service = scope.ServiceProvider.GetRequiredService<IClientPresenceService>();

        var data = await service.GetConnectedClients(token);

        foreach (var info in data)
        {
            await _messageBus.Publish(info, token);
        }

        _logger.LogDebug("Published presence info for {DataCount} clients", data.Count);
    }
}