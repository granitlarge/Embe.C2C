using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications;

public class MatchingCreatedConfiguration : AggregateConfiguration<MatchingCreated>
{
    public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<MatchingCreated> builder)
    {
        builder.HasBaseType<Notification>();
        builder.HasOne<Matching>()
            .WithMany()
            .HasForeignKey(n => n.MatchingId)
            .OnDelete(DeleteBehavior.ClientCascade);
        base.Configure(builder);
    }
}