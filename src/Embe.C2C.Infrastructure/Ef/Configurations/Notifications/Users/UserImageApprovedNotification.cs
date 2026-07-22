using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Notifications.Users;
using Embe.C2C.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications.Users;

public class UserImageApprovedNotificationConfiguration : IEntityTypeConfiguration<UserImageApprovedNotification>
{
    public void Configure(EntityTypeBuilder<UserImageApprovedNotification> builder)
    {
        builder.HasBaseType<Notification>();
        builder.HasOne<Image>()
            .WithMany()
            .HasForeignKey(n => n.ImageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(n => n.OriginalImageUrl);
        builder.Property(n => n.LargeImageUrl);
        builder.Property(n => n.MediumImageUrl);
        builder.Property(n => n.SmallImageUrl);
    }
}