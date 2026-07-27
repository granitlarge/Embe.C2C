using Embe.C2C.Domain.Aggregates.Notifications.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications.Messages;

public class MessageCreatedConfiguration : IEntityTypeConfiguration<MessageCreatedNotification>
{
    public void Configure(EntityTypeBuilder<MessageCreatedNotification> builder)
    {
        builder.HasBaseType<MessageNotification>();
    }
}