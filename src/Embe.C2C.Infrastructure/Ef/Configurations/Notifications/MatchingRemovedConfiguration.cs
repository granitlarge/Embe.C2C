using Embe.C2C.Domain.Aggregates.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications;

public class MatchingRemovedConfiguration : IEntityTypeConfiguration<MatchingRemoved>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<MatchingRemoved> builder)
    {
        builder.HasBaseType<Notification>();
    }
}