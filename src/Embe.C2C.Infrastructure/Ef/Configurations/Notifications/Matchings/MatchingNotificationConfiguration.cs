using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Notifications.Matchings;
using Embe.C2C.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications.Matchings;

public class MatchingNotificationConfiguration : IEntityTypeConfiguration<MatchingNotification>
{
    public void Configure(EntityTypeBuilder<MatchingNotification> builder)
    {
        builder.HasBaseType<Notification>();
        builder.HasOne<Matching>()
            .WithMany()
            .HasForeignKey(n => n.MatchingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
        .HasOne<User>()
            .WithMany()
            .HasForeignKey(n => n.PartnerUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}