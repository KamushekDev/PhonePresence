using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PresenceBot.Core.Presence;
using PresenceBot.Infrastructure;
using PresenceBot.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services
    .AddHealthChecks()
    .AddCheck("live", _ => HealthCheckResult.Healthy());

// builder.Services.AddHostedService<>();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();


using (var scope = ((IApplicationBuilder)app).ApplicationServices.CreateScope())
{
    using (var context = scope.ServiceProvider.GetRequiredService<PresenceContext>())
        context.Database.Migrate();
}

app.MapHealthChecks("live");

app.MapGet("client",
    async (IClientPresenceRepository repository, CancellationToken token) =>
        await repository.GetClients(token));

app.MapGet("client/{identity}",
    async (string identity, IClientPresenceRepository repository, CancellationToken token) =>
        await repository.FindByIdentity(identity, token));

app.Run();