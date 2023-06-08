using Microsoft.EntityFrameworkCore;
using PresenceBot.Core.Presence;
using PresenceBot.Core.Presence.Models;
using PresenceBot.Infrastructure.Database;
using PresenceBot.Infrastructure.Database.Models;

namespace PresenceBot.Infrastructure.Presence;

public class ClientPresenceRepository : IClientPresenceRepository
{
    private readonly PresenceContext _context;

    public ClientPresenceRepository(PresenceContext context)
    {
        _context = context;
    }

    public async Task AddClientPresence(CreateClientPresence request, CancellationToken token)
    {
        var model = MapToModel(request);
        await _context.Clients.AddAsync(model, token);
        await _context.SaveChangesAsync(token);

        static ClientPresenceModel MapToModel(CreateClientPresence req) => new()
        {
            Identity = req.Identity,
            IdentityComponents = req.IdentityComponents,
            LastPresentedAt = req.LastAvailableAt
        };
    }

    public async Task<ClientPresence?> UpdateLastAvailableAt(string identity, DateTimeOffset newValue,
        CancellationToken token)
    {
        var value = await _context.Clients.FirstOrDefaultAsync(x => x.Identity == identity, token);
        if (value is null)
            return null;
        value.LastPresentedAt = newValue;
        await _context.SaveChangesAsync(token);
        return ToCoreModel(value);
    }

    public async Task<ClientPresence?> FindByIdentity(string identity, CancellationToken token)
    {
        var value = await _context.Clients.AsNoTracking().FirstOrDefaultAsync(x => x.Identity == identity, token);
        return value is null ? null : ToCoreModel(value);
    }

    public async Task<List<ClientPresence>> GetClients(CancellationToken token)
    {
        var models = await _context.Clients.AsNoTracking().ToListAsync(token);
        return models.Select(ToCoreModel).ToList();
    }

    private static ClientPresence ToCoreModel(ClientPresenceModel model) => new()
    {
        Identity = model.Identity,
        LastAvailableAt = model.LastPresentedAt
    };
}