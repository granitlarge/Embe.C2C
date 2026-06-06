using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);
        builder.HasDiscriminator<string>("NotificationType")
            .HasValue<MatchingCreated>("MatchingCreated")
            .HasValue<MatchingRemoved>("MatchingRemoved");

        builder.HasOne<User>()
             .WithMany()
             .HasForeignKey(n => n.RecipientUserId)
             .OnDelete(DeleteBehavior.Cascade);
    }
}