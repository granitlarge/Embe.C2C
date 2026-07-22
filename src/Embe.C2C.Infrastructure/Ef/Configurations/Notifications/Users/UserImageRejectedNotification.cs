using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Notifications.Matchings;
using Embe.C2C.Domain.Aggregates.Notifications.Users;
using Embe.C2C.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications.Users;

public class UserImageRejectedNotificationConfiguration : IEntityTypeConfiguration<UserImageRejectedNotification>
{
    public void Configure(EntityTypeBuilder<MatchingNotification> builder)
    {

    }

    public void Configure(EntityTypeBuilder<UserImageRejectedNotification> builder)
    {
        builder.HasBaseType<Notification>();
        builder.HasOne<Image>()
            .WithMany()
            .HasForeignKey(n => n.ImageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}