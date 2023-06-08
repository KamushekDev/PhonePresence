using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PresenceBot.Core.MessageBus;
using PresenceBot.Core.Presence;
using PresenceBot.Core.Presence.Models;

namespace PresenceBot.Infrastructure.BackgroundJobs;

public class PresenceInfoHandlerJob : IHostedService
{
    private readonly IMessageBus<PresenceInfo> _messageBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PresenceInfoHandlerJob> _logger;

    private Task? _worker;
    private CancellationTokenSource? _cts;

    public PresenceInfoHandlerJob(IMessageBus<PresenceInfo> messageBus, IServiceProvider serviceProvider,
        ILogger<PresenceInfoHandlerJob> logger)
    {
        _messageBus = messageBus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new TaskFactory();
        _cts = new CancellationTokenSource();
        _worker = factory.StartNew(() => Worker(_cts.Token), _cts.Token);

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
                await WorkerInternal(token);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong on handling presence info");
            }
        }
    }

    private async Task WorkerInternal(CancellationToken token)
    {
        var message = await _messageBus.Consume(token);
        if (!message.Available)
        {
            _logger.LogDebug("Skipped presence info for {Identity}", message.Client.Identity);
            return;
        }

        var repository = _serviceProvider
            .CreateAsyncScope()
            .ServiceProvider
            .GetRequiredService<IClientPresenceRepository>();

        var client = await repository.FindByIdentity(message.Client.Identity, token);
        if (client is null)
        {
            var request = new CreateClientPresence()
            {
                Identity = message.Client.Identity,
                LastAvailableAt = message.Moment,
                IdentityComponents = new[] { message.Client.Nickname, message.Client.Name, message.Client.Mac }
            };
            await repository.AddClientPresence(request, token);
            _logger.LogDebug("Created new client entry for {Identity}", message.Client.Identity);
        }
        else
        {
            await repository.UpdateLastAvailableAt(client.Identity, message.Moment, token);
            _logger.LogDebug("Updated client entry for {Identity}", message.Client.Identity);
        }
    }
}