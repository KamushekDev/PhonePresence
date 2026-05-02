using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PresenceBot.Core.Notifications.Models;
using PresenceBot.Infrastructure.Database.Models;

namespace PresenceBot.Infrastructure.Database.Configs;

public class NotificationRequestConfiguration : IEntityTypeConfiguration<NotificationRequest>
{
    public void Configure(EntityTypeBuilder<NotificationRequest> builder)
    {
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}