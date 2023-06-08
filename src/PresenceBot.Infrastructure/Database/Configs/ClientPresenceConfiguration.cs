using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using PresenceBot.Infrastructure.Database.Models;

namespace PresenceBot.Infrastructure.Database.Configs;

public class ClientPresenceConfiguration : IEntityTypeConfiguration<ClientPresenceModel>
{
    public void Configure(EntityTypeBuilder<ClientPresenceModel> builder)
    {
        builder
            .HasKey(x => x.Identity);
        builder
            .Property(x => x.IdentityComponents)
            .HasConversion(
                x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<string[]>(x) ?? Array.Empty<string>()
            );
    }
}