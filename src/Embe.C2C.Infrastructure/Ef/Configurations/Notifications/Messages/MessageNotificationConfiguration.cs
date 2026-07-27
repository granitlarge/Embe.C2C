using Embe.C2C.Domain.Aggregates.Messages;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Notifications.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications.Messages;

public class MessageNotificationConfiguration : IEntityTypeConfiguration<MessageNotification>
{
    public void Configure(EntityTypeBuilder<MessageNotification> builder)
    {
        builder.HasBaseType<Notification>();
        builder.HasOne<Message>()
            .WithMany()
            .HasForeignKey(n => n.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}