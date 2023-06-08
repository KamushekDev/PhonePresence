using Microsoft.EntityFrameworkCore;
using PresenceBot.Infrastructure.Database.Models;

namespace PresenceBot.Infrastructure.Database;

public class PresenceContext : DbContext
{
    public PresenceContext(DbContextOptions<PresenceContext> options) : base(options) { }

    public required DbSet<ClientPresenceModel> Clients { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IInfrastructureMarker).Assembly);
    }
}