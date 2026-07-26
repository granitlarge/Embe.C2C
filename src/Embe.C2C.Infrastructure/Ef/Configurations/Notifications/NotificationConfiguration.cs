using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Notifications.Matchings;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications;

public class NotificationConfiguration : AggregateConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);
        builder.HasDiscriminator<string>("NotificationType")
            .HasValue<MatchingCreated>("MatchingCreated");

        builder.HasOne<User>()
             .WithMany()
             .HasForeignKey(n => n.RecipientUserId)
             .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(n => n.ReadAt);

        base.Configure(builder);
    }
}