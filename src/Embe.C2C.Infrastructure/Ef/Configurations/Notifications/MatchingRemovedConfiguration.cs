using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications;

public class MatchingRemovedConfiguration : AggregateConfiguration<MatchingRemoved>
{
    public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<MatchingRemoved> builder)
    {
        builder.HasBaseType<Notification>();
        base.Configure(builder);
    }
}