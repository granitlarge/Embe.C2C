using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Notifications.Candidates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications.Candidates;

public class PositivelyJudgedConfiguration : IEntityTypeConfiguration<PositivelyJudgedNotification>
{
    public void Configure(EntityTypeBuilder<PositivelyJudgedNotification> builder)
    {
        builder.HasBaseType<Notification>();
    }
}