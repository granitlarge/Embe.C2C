using Embe.C2C.Domain.Aggregates.Notifications.Matchings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications.Matchings;

public class MatchingCreatedConfiguration : IEntityTypeConfiguration<MatchingCreated>
{
    public void Configure(EntityTypeBuilder<MatchingCreated> builder)
    {
        builder.HasBaseType<MatchingNotification>();
    }
}